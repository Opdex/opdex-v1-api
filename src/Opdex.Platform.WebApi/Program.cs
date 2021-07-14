using System;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Opdex.Platform.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // This is just the static global logger used for the host process before DI registrations and container setup has occurred.
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateBootstrapLogger();

            try
            {
                Log.Information("Starting Platform Api");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Platform Api application start-up failed");
            }

            Log.CloseAndFlush();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .UseSerilog((context, services, loggingConfiguration) =>
                {
                    loggingConfiguration
                        .ReadFrom.Configuration(context.Configuration)
                        .WriteTo.ApplicationInsights(services.GetRequiredService<TelemetryConfiguration>(), TelemetryConverter.Traces);
                })
                .ConfigureServices(services =>
                {
                    services.AddHostedService<IndexerBackgroundService>();
                });
    }
}
