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

namespace Opdex.Platform.WebApi
{
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
                options.ShouldLogUnhandledException = (context, exception, problem) => problem.Status >= 400;
                options.Map<BadRequestException>(e => new StatusCodeProblemDetails(StatusCodes.Status400BadRequest) { Detail = e.Message });
                options.Map<InvalidDataException>(e => ValidationErrorProblemDetailsResult.CreateProblemDetails(e.PropertyName, e.Message));
                options.Map<IndexingAlreadyRunningException>(e => new StatusCodeProblemDetails(StatusCodes.Status503ServiceUnavailable) { Detail = e.Message });
                options.Map<NotFoundException>(e => new StatusCodeProblemDetails(StatusCodes.Status404NotFound) { Detail = e.Message });
                options.MapToStatusCode<NotImplementedException>(StatusCodes.Status501NotImplemented);
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

            // Startup Configuration Validation Filters
            services.AddTransient<IStartupFilter, ConfigurationValidationStartupFilter>();

            // Cirrus Configurations
            var cirrusConfig = Configuration.GetSection(nameof(CirrusConfiguration));
            services.SetupConfiguration<CirrusConfiguration>(cirrusConfig);

            // Opdex Configurations
            var opdexConfig = Configuration.GetSection(nameof(OpdexConfiguration));
            services.SetupConfiguration<OpdexConfiguration>(opdexConfig);

            // Database Configurations
            var databaseConfig = Configuration.GetSection(nameof(DatabaseConfiguration));
            services.SetupConfiguration<DatabaseConfiguration>(databaseConfig);

            // Coin Market Cap Configurations
            var cmcConfig = Configuration.GetSection(nameof(CoinMarketCapConfiguration));
            services.SetupConfiguration<CoinMarketCapConfiguration>(cmcConfig);

            var authConfig = Configuration.GetSection(nameof(AuthConfiguration));
            services.SetupConfiguration<AuthConfiguration>(authConfig);

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
                            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                            {
                                // assigns the bearer token from signalR connection
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
                settings.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor());
                settings.TypeMappers.Add(new PrimitiveTypeMapper(typeof(Address), schema => schema.Type = JsonObjectType.String));
                settings.TypeMappers.Add(new PrimitiveTypeMapper(typeof(FixedDecimal), schema =>
                {
                    schema.IsNullableRaw = false;
                    schema.Type = JsonObjectType.String;
                }));
                settings.TypeMappers.Add(new PrimitiveTypeMapper(typeof(Sha256), schema =>
                {
                    schema.IsNullableRaw = false;
                    schema.Type = JsonObjectType.String;
                    schema.Pattern = @"^[0-9a-fA-F]{64}$";
                }));
            });

            services.AddSignalR()
                    .AddAzureSignalR();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.Requirements.Add(new AdminOnlyRequirement()));
            });

            services.AddSingleton<IUserIdProvider, WalletAddressUserIdProvider>();
            services.AddSingleton<IAuthorizationHandler, AdminOnlyHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                IdentityModelEventSource.ShowPII = true;
            }

            app.UseProblemDetails();
            app.UseMiddleware<RedirectToResourceMiddleware>();
            app.UseCors(options => options
                            .SetIsOriginAllowed(host => true)
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials());
            app.UseSerilogRequestLogging();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseOpenApi();
            app.UseSwaggerUi3();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<PlatformHub>("/transactions/socket");
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
}
