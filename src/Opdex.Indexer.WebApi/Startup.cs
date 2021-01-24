using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Opdex.Core.Application;
using Opdex.Core.Application.Abstractions;
using Opdex.Core.Common;
using Opdex.Core.Infrastructure;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi;
using Opdex.Core.Infrastructure.Clients;
using Opdex.Indexer.Application;
using Opdex.Indexer.Application.Abstractions;
using Opdex.Indexer.Infrastructure;
using Serilog;

namespace Opdex.Indexer.WebApi
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
            
            services.AddOpenApiDocument(settings =>
            {
                settings.Title = "Opdex Platform API";
                settings.Version = "v1";
            });
            
            // Automapper Profiles
            services.AddAutoMapper(mapperConfig =>
            {
                mapperConfig.AddProfile<CoreInfrastructureMapperProfile>();
                mapperConfig.AddProfile<IndexerInfrastructureMapperProfile>();
            });
            
            services.AddHostedService<IndexerBackgroundService>();
            
            services.AddHttpClient();

            services.AddTransient<IIndexProcessManager, IndexProcessManager>();

            // Todo: Maybe not needed
            var cirrusConfiguration = Configuration.GetSection(nameof(CirrusConfiguration));
            services.Configure<CirrusConfiguration>(cirrusConfiguration);
            
            var opdexConfig = Configuration.GetSection(nameof(OpdexConfiguration));
            services.Configure<OpdexConfiguration>(opdexConfig);
            
            // Register project module services
            services.AddCoreInfrastructureServices(cirrusConfiguration.Get<CirrusConfiguration>());
            services.AddCoreApplicationServices();
            services.AddIndexerInfrastructureServices();
            services.AddIndexerApplicationServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseSerilogRequestLogging();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            
            app.UseOpenApi();
            
            app.UseSwaggerUi3();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}