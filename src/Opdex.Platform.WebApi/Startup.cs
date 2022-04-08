using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Opdex.Platform.Application;
using Opdex.Platform.Infrastructure;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi;
using Opdex.Platform.WebApi.Mappers;
using Serilog;
using Hellang.Middleware.ProblemDetails;
using System;
using Microsoft.AspNetCore.Http;
using Opdex.Platform.Common.Exceptions;
using Hellang.Middleware.ProblemDetails.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Opdex.Platform.WebApi.Auth;
using Microsoft.IdentityModel.Logging;
using System.Text;
using CcAcca.ApplicationInsights.ProblemDetails;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.AspNetCore.Authorization;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.WebApi.Extensions;
using Opdex.Platform.WebApi.Middleware;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.WebApi.Models.Binders;
using Opdex.Platform.Common;
using Opdex.Platform.WebApi.Models.Responses;
using Opdex.Platform.Infrastructure.Clients.SignalR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using FluentValidation.AspNetCore;
using AspNetCoreRateLimit;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Opdex.Platform.WebApi.Exceptions;
using Opdex.Platform.Common.Encryption;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCaching;
using Opdex.Platform.WebApi.Conventions;
using Microsoft.Extensions.FileProviders;
using System.Reflection;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.FeatureManagement;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;

namespace Opdex.Platform.WebApi;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddApplicationInsightsTelemetry(new ApplicationInsightsServiceOptions
        {
            // disable default adaptive sampling configuration, instead we customise this in Configure
            EnableAdaptiveSampling = false
        });

        // gets rid of telemetry spam in the debug console, may prevent visual studio app insights monitoring
        TelemetryDebugWriter.IsTracingDisabled = true;

        services.AddFeatureManagement();

        services.AddProblemDetails(options =>
        {
            options.ValidationProblemStatusCode = 400;
            options.ShouldLogUnhandledException = (httpContext, exception, _) =>
            {
                httpContext.Items.Add("UnhandledException", exception);
                return false;
            };
            options.Map<InvalidDataException>(e => ProblemDetailsTemplates.CreateValidationProblemDetails(e.PropertyName, e.Message));
            options.Map<AlreadyIndexedException>(e => new StatusCodeProblemDetails(StatusCodes.Status400BadRequest) { Detail = e.Message });
            options.Map<NotAllowedException>(e => new StatusCodeProblemDetails(StatusCodes.Status403Forbidden) { Detail = e.Message });
            options.Map<NotFoundException>(e => new StatusCodeProblemDetails(StatusCodes.Status404NotFound) { Detail = e.Message });
            options.Map<TooManyRequestsException>((context, exception) =>
            {
                context.Response.Headers["Retry-After"] = exception.RetryAfter;
                return new StatusCodeProblemDetails(StatusCodes.Status429TooManyRequests)
                {
                    Detail = $"Quota exceeded. Maximum allowed: {exception.Limit} per {exception.Period}. Please try again in {exception.RetryAfter} second(s)."
                };
            });
            options.MapToStatusCode<NotImplementedException>(StatusCodes.Status501NotImplemented);
            options.Map<IndexingAlreadyRunningException>(e => new StatusCodeProblemDetails(StatusCodes.Status503ServiceUnavailable) { Detail = e.Message });
            options.Map<MaintenanceLockException>(_ => new StatusCodeProblemDetails(StatusCodes.Status503ServiceUnavailable) { Detail = "Currently undergoing maintenance" });
            options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
            options.IncludeExceptionDetails = (context, _) =>
            {
                var environment = context.RequestServices.GetRequiredService<IHostEnvironment>();
                return environment.IsDevelopment();
            };
        });

        var serializerSettings = Serialization.DefaultJsonSettings;

        services
            .AddControllers(options =>
            {
                options.ModelBinderProviders.Insert(0, new AddressModelBinderProvider());
                options.ModelBinderProviders.Insert(1, new Sha256ModelBinderProvider());
            })
            .AddFluentValidation(config =>
            {
                config.DisableDataAnnotationsValidation = true;
                config.RegisterValidatorsFromAssemblyContaining<Startup>();
            })
            .AddNetworkActionHidingConvention()
            .AddProblemDetailsConventions()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.NullValueHandling = serializerSettings.NullValueHandling;
                options.SerializerSettings.ContractResolver = serializerSettings.ContractResolver;
                options.SerializerSettings.Converters = serializerSettings.Converters;
            });

        JsonConvert.DefaultSettings = () => serializerSettings;

        services.AddProblemDetailTelemetryInitializer();

        services.AddHttpClient();

        // Automapper
        services.AddTransient<TransactionErrorMappingAction>();
        services.AddAutoMapper(mapperConfig =>
        {
            mapperConfig.AddProfile<PlatformApplicationMapperProfile>();
            mapperConfig.AddProfile<PlatformInfrastructureMapperProfile>();
            mapperConfig.AddProfile<PlatformWebApiMapperProfile>();
        });

        // Rate Limiting
        services.AddMemoryCache();
        services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"))
                .Configure<IpRateLimitOptions>(options =>
                {
                    options.RequestBlockedBehaviorAsync = (_, _, rateLimitCounter, rule) =>
                    {
                        var retryAfter = rateLimitCounter.Timestamp.RetryAfterFrom(rule);
                        throw new TooManyRequestsException(rule.Limit, rule.Period, retryAfter);
                    };
                });
        services.Configure<IpRateLimitPolicies>(Configuration.GetSection("IpRateLimitPolicies"));
        services.AddInMemoryRateLimiting();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

        // Startup Configuration Validation Filters
        services.AddTransient<IStartupFilter, ConfigurationValidationStartupFilter>();

        // Cirrus Configurations
        var cirrusConfig = Configuration.GetSection(nameof(CirrusConfiguration));
        services.SetupConfiguration<CirrusConfiguration>(cirrusConfig);

        // Opdex Configurations
        var opdexConfig = Configuration.GetSection(nameof(OpdexConfiguration));
        services.SetupConfiguration<OpdexConfiguration>(opdexConfig);

        // Encryption Configurations
        var encryptionConfig = Configuration.GetSection(nameof(EncryptionConfiguration));
        services.SetupConfiguration<EncryptionConfiguration>(encryptionConfig);

        // Database Configurations
        var databaseConfig = Configuration.GetSection(nameof(DatabaseConfiguration));
        services.SetupConfiguration<DatabaseConfiguration>(databaseConfig);

        // Coin Market Cap Configurations
        var cmcConfig = Configuration.GetSection(nameof(CoinMarketCapConfiguration));
        services.SetupConfiguration<CoinMarketCapConfiguration>(cmcConfig);

        // Auth Configurations
        var authConfig = Configuration.GetSection(nameof(AuthConfiguration));
        services.SetupConfiguration<AuthConfiguration>(authConfig);

        services.Configure<IndexerConfiguration>(Configuration.GetSection(nameof(IndexerConfiguration)));

        // Maintenance Configurations
        services.Configure<MaintenanceConfiguration>(Configuration.GetSection(nameof(MaintenanceConfiguration)));
        services.AddScoped<MaintenanceLockFilter>();

        // Register project module services
        services.AddPlatformApplicationServices();
        services.AddPlatformInfrastructureServices(cirrusConfig.Get<CirrusConfiguration>(),
                                                   cmcConfig.Get<CoinMarketCapConfiguration>());

        services.AddScoped<IApplicationContext, ApplicationContext>();

        // Fixes missing claims such as "sub" - https://leastprivilege.com/2017/11/15/missing-claims-in-the-asp-net-core-2-openid-connect-handler/
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(async options =>
            {
                if (Configuration.GetValue<bool?>("FeatureManagement:AuthServer") ?? false)
                {
                    var issuer = Configuration["AuthConfiguration:Issuer"];

                    using var httpClient = new HttpClient();
                    var jwksResponse = await httpClient.GetAsync($"https://{issuer}/v1/auth/keys");
                    var jwks = await jwksResponse.Content.ReadAsStringAsync();

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = issuer,
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKeys = new JsonWebKeySet(jwks).GetSigningKeys(),
                        ValidateLifetime = true,
                    };
                }
                else
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authConfig.Get<AuthConfiguration>().Opdex.SigningKey))
                    };
                }

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        // signalR sends bearer token in the query string
                        var accessToken = context.Request.Query["access_token"];

                        var path = context.HttpContext.Request.Path;

                        // Note: Sharing a hub has side effects for auth
                        // We can separate platform (requiring jwt) from auth (providing jwt) to two hubs requiring > free tier of Azure Signalr
                        // Current implementation will pass through if access_token is not found and client doesn't negotiate the connection.
                        // Doesn't raise security concerns with current notification based use-cases as no access_token would mean no transaction
                        // notifications when connected due to no access_token w/ wallet address claim.
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/socket"))
                        {
                            // assigns the bearer token from signalR connection if available
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        services.AddResponseCaching();

        services.AddApiVersioning(options =>
        {
            options.ReportApiVersions = true;
            options.ErrorResponses = new ProblemDetailsApiVersionErrorProvider();
            options.DefaultApiVersion = new ApiVersion(1, 0);
        });

        services.AddSignalR(o => { o.EnableDetailedErrors = true; }).AddAzureSignalR();

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.Requirements.Add(new AdminOnlyRequirement()));
        });

        services.AddSingleton<IUserIdProvider, WalletAddressUserIdProvider>();
        services.AddScoped<IAuthorizationHandler, AdminOnlyHandler>();

        services.AddTransient<ITwoWayEncryptionProvider, AesCbcProvider>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, TelemetryConfiguration configuration)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            IdentityModelEventSource.ShowPII = true;
        }
        else
        {
            app.UseHsts();
            app.UseHttpsRedirection();
        }

        var builder = configuration.DefaultTelemetrySink.TelemetryProcessorChainBuilder;
        builder.UseAdaptiveSampling(maxTelemetryItemsPerSecond:5, excludedTypes:"Exception");
        builder.Use(next => new IgnoreRequestPathsTelemetryProcessor(next));
        builder.Build();

        app.UseSerilogRequestLogging(options =>
        {
            options.EnrichDiagnosticContext += (diagnosticContext, httpContext) =>
            {
                if (httpContext.Items.TryGetValue("UnhandledException", out var exception))
                {
                    diagnosticContext.SetException((Exception)exception);
                }
            };
        });
        app.UseProblemDetails();
        app.UseMiddleware<RedirectToResourceMiddleware>();
        app.UseCors(options => options
                        .SetIsOriginAllowed(host => true)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials());
        app.UseRouting();
        app.UseIpRateLimiting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseResponseCaching();
        app.Use((context, next) =>
        {
            var responseCachingFeature = context.Features.Get<IResponseCachingFeature>();
            // prevent cached responses when query params change
            if (responseCachingFeature is not null) responseCachingFeature.VaryByQueryKeys = new []{ "*" };
            return next();
        });

        // yaml mapping not supported by default, must explicitly map
        var fileExtensionContentTypeProvider = new FileExtensionContentTypeProvider();
        fileExtensionContentTypeProvider.Mappings.Add(".yml", "text/yaml");
        app.UseStaticFiles(new StaticFileOptions
        {
            ContentTypeProvider = fileExtensionContentTypeProvider,
            FileProvider = new ManifestEmbeddedFileProvider(Assembly.GetExecutingAssembly()),
            RequestPath = "/swagger/v1"
        });

        app.UseSwaggerUI(options =>
        {
            options.RoutePrefix = "swagger";
            options.InjectJavascript("v1/openapi.js");
        });

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHub<PlatformHub>("/v1/socket");
            endpoints.MapControllers();
        });
    }

    /// <summary>
    /// Ignores telemetry from non-api paths.
    /// </summary>
    public class IgnoreRequestPathsTelemetryProcessor : ITelemetryProcessor
    {
        private readonly ITelemetryProcessor _next;

        public IgnoreRequestPathsTelemetryProcessor(ITelemetryProcessor next)
        {
            _next = next;
        }

        public void Process(ITelemetry item)
        {
            if (item is RequestTelemetry request &&
                (request.Url.AbsolutePath == "/" ||
                 request.Url.AbsolutePath == "/favicon.ico" ||
                 request.Url.AbsolutePath.StartsWith("/swagger")))
            {
                return;
            }

            _next.Process(item);
        }
    }
}
