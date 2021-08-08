using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Opdex.Platform.Application;
using Opdex.Platform.Infrastructure;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi;
using Opdex.Platform.WebApi.Mappers;
using Serilog;
using System.Collections.Generic;
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
using Microsoft.Net.Http.Headers;
using NSwag.Generation.Processors.Security;
using System.Text;
using CcAcca.ApplicationInsights.ProblemDetails;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.WebApi.Extensions;
using Opdex.Platform.WebApi.Middleware;
using Opdex.Platform.WebApi.Models;

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
                options.ShouldLogUnhandledException = (context, exception, problem) => problem.Status >= 400;
                options.Map<BadRequestException>(e => new StatusCodeProblemDetails(StatusCodes.Status400BadRequest) { Detail = e.Message });
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

            services
                .AddControllers()
                .AddProblemDetailsConventions()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.Converters =
                        new List<JsonConverter>
                        {
                            new StringEnumConverter(),
                            new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd'T'HH:mm:ssK" }
                        };
                });

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

            // Block Explorer Configurations
            var blockExplorerConfig = Configuration.GetSection(nameof(BlockExplorerConfiguration));
            services.SetupConfiguration<BlockExplorerConfiguration>(blockExplorerConfig);

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
                });

            services.AddOpenApiDocument(settings =>
            {
                settings.Title = "Opdex Platform API";
                settings.Version = "v1";
                settings.AddSecurity(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Description = "Type into textbox: Bearer [your jwt]",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Name = HeaderNames.Authorization,
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                });
                settings.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor());
            });
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
            app.UseCors(options => options
                            .AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod());
            app.UseSerilogRequestLogging();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseOpenApi();
            app.UseSwaggerUi3();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
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
