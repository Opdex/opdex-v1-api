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
using Opdex.Platform.Common;
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
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.Converters =
                        new List<JsonConverter>
                        {
                            new StringEnumConverter(),
                            new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd'T'HH:mm:ss.fffK" }
                        };
                });

            var authConfig = Configuration.GetSection(nameof(AuthConfiguration));
            services.Configure<AuthConfiguration>(authConfig);

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

            // Automapper Profiles
            services.AddAutoMapper(mapperConfig =>
            {
                mapperConfig.AddProfile<PlatformApplicationMapperProfile>();
                mapperConfig.AddProfile<PlatformInfrastructureMapperProfile>();
                mapperConfig.AddProfile<PlatformWebApiMapperProfile>();
            });

            services.AddHttpClient();

            var cirrusConfig = Configuration.GetSection(nameof(CirrusConfiguration));
            services.Configure<CirrusConfiguration>(cirrusConfig);

            var opdexConfig = Configuration.GetSection(nameof(OpdexConfiguration));
            services.Configure<OpdexConfiguration>(opdexConfig);

            var cmcConfig = Configuration.GetSection(nameof(CoinMarketCapConfiguration));
            services.Configure<CoinMarketCapConfiguration>(cmcConfig);

            // Register project module services
            services.AddPlatformApplicationServices();
            services.AddPlatformInfrastructureServices(cirrusConfig.Get<CirrusConfiguration>(), cmcConfig.Get<CoinMarketCapConfiguration>());
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

            // Todo: Set correctly for ENV's outside local dev
            app.UseCors(options => options
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());

            app.UseSerilogRequestLogging();

            // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseOpenApi();

            app.UseSwaggerUi3();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}