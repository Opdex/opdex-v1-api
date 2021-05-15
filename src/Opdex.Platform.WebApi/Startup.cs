using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
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
            services
                .AddControllers()
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

            services.AddControllers(o =>
            {
                o.Conventions.Add(new ActionHidingConvention());
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

            // Todo: Set correctly for ENV's outside local dev
            app.UseCors(options => options
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());

            app.UseSerilogRequestLogging();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseOpenApi();

            app.UseSwaggerUi3();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        /// <summary>
        /// Hide Stratis related endpoints in Swagger shown due to using Nuget packages
        /// in WebApi project for serialization.
        /// </summary>
        public class ActionHidingConvention : IActionModelConvention
        {
            public void Apply(ActionModel action)
            {
                // Replace with any logic you want
                if (!action.Controller.DisplayName.Contains("Opdex"))
                {
                    action.ApiExplorer.IsVisible = false;
                }
            }
        }
    }
}