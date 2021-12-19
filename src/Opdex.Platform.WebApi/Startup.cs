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
using NSwag;
using NSwag.Generation.Processors.Security;
using System.Text;
using CcAcca.ApplicationInsights.ProblemDetails;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.AspNetCore.Authorization;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Converters;
using Opdex.Platform.WebApi.Extensions;
using Opdex.Platform.WebApi.Middleware;
using Opdex.Platform.WebApi.Models;
using System.ComponentModel;
using Opdex.Platform.Common.Models;
using NJsonSchema.Generation.TypeMappers;
using NJsonSchema;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.WebApi.Models.Binders;
using Opdex.Platform.Common;
using Opdex.Platform.WebApi.Models.Responses;
using Opdex.Platform.Infrastructure.Clients.SignalR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using FluentValidation.AspNetCore;
using Opdex.Platform.WebApi.Validation;
using AspNetCoreRateLimit;
using Opdex.Platform.WebApi.Exceptions;
using Opdex.Platform.Common.Encryption;
using Opdex.Platform.WebApi.OpenApi;

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
        services.AddApplicationInsightsTelemetry();
        services.AddApplicationInsightsTelemetryProcessor<IgnoreRequestPathsTelemetryProcessor>();

        // gets rid of telemetry spam in the debug console, may prevent visual studio app insights monitoring
        TelemetryDebugWriter.IsTracingDisabled = true;

        services.AddProblemDetails(options =>
        {
            options.ValidationProblemStatusCode = 400;
            // Serilog.AspNetCore.RequestLoggingMiddleware does this better although exception is lost
            // See https://github.com/serilog/serilog-aspnetcore/issues/270
            options.ShouldLogUnhandledException = (context, exception, problem) => problem.Status == 500;
            options.Map<InvalidDataException>(e => ProblemDetailsTemplates.CreateValidationProblemDetails(e.PropertyName, e.Message));
            options.Map<AlreadyIndexedException>(e => new StatusCodeProblemDetails(StatusCodes.Status400BadRequest) { Detail = e.Message });
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
            options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
            options.IncludeExceptionDetails = (context, ex) =>
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

        // Automapper Profiles
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
                options.RequestBlockedBehaviorAsync = (context, identity, rateLimitCounter, rule) =>
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

        var authConfig = Configuration.GetSection(nameof(AuthConfiguration));
        services.SetupConfiguration<AuthConfiguration>(authConfig);

        services.Configure<IndexerConfiguration>(Configuration.GetSection(nameof(IndexerConfiguration)));

        // Register project module services
        services.AddPlatformApplicationServices();
        services.AddPlatformInfrastructureServices(cirrusConfig.Get<CirrusConfiguration>(),
                                                   cmcConfig.Get<CoinMarketCapConfiguration>());

        services.AddScoped<IApplicationContext, ApplicationContext>();


        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    // temp solution, OAuth will prefer assymmetric key
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authConfig.Get<AuthConfiguration>().Opdex.SigningKey))
                };
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

        services.AddOpenApiDocument((settings, provider) =>
        {
            // must add type converter attribute to pass NSwag check for IsPrimitiveType
            TypeDescriptor.AddAttributes(typeof(Address), new TypeConverterAttribute(typeof(AddressConverter)));
            TypeDescriptor.AddAttributes(typeof(FixedDecimal), new TypeConverterAttribute(typeof(FixedDecimalConverter)));
            TypeDescriptor.AddAttributes(typeof(Sha256), new TypeConverterAttribute(typeof(Sha256)));

            // processes fluent validation rules as OpenAPI type rules
            settings.AddFluentValidationSchemaProcessor(provider, config =>
            {
                config.RegisterRulesFromAssemblyContaining<Startup>();
            });

            settings.Title = "Opdex Platform API";
            settings.Version = "v1";
            settings.AddSecurity(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            {
                Description = "Enter your JWT.",
                Type = OpenApiSecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });
            settings.DocumentProcessors.Add(new SingleAllOfToRefDocumentProcessor());
            settings.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor());
            settings.OperationProcessors.Add(new TooManyRequestErrorOperationProcessor());
            settings.OperationProcessors.Add(new InternalServerErrorOperationProcessor());
            settings.TypeMappers.Add(new PrimitiveTypeMapper(typeof(Address), schema => schema.Type = JsonObjectType.String));
            settings.TypeMappers.Add(new PrimitiveTypeMapper(typeof(FixedDecimal), schema =>
            {
                schema.Type = JsonObjectType.String;
            }));
            settings.TypeMappers.Add(new PrimitiveTypeMapper(typeof(Sha256), schema =>
            {
                schema.Type = JsonObjectType.String;
                schema.Pattern = @"^[0-9a-fA-F]{64}$";
            }));
        });

        services.AddSignalR(o => { o.EnableDetailedErrors = true; }).AddAzureSignalR();

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.Requirements.Add(new AdminOnlyRequirement()));
        });

        services.AddSingleton<IUserIdProvider, WalletAddressUserIdProvider>();
        services.AddSingleton<IAuthorizationHandler, AdminOnlyHandler>();

        services.AddTransient<ITwoWayEncryptionProvider, AesCbcProvider>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            IdentityModelEventSource.ShowPII = true;
        }

        app.UseSerilogRequestLogging();
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
        app.UseOpenApi();
        app.UseSwaggerUi3();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHub<PlatformHub>("/socket");
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
