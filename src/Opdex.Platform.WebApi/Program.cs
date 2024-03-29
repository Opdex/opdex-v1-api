using System;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Opdex.Platform.Common.Configurations;
using Serilog;

namespace Opdex.Platform.WebApi;

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

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                if (!context.HostingEnvironment.IsProduction()) return;

                var builtConfig = config.Build();
                var secretClient = new SecretClient(
                    new Uri($"https://{builtConfig["Azure:KeyVault:Name"]}.vault.azure.net/"),
                    new DefaultAzureCredential());

                config.AddAzureKeyVault(secretClient, new AzureKeyVaultConfigurationOptions
                {
                    Manager = new KeyVaultSecretManager(),
                    ReloadInterval = TimeSpan.FromSeconds(16)
                });
            })
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
            .UseSerilog((context, services, loggingConfiguration) =>
            {
                var opdexConfiguration = services.GetRequiredService<OpdexConfiguration>();
                loggingConfiguration
                    .Enrich.WithProperty("InstanceId", opdexConfiguration.InstanceId)
                    .ReadFrom.Configuration(context.Configuration)
                    .WriteTo.ApplicationInsights(services.GetRequiredService<TelemetryConfiguration>(), TelemetryConverter.Traces);
            })
            .ConfigureServices(services =>
            {
                services.AddHostedService<IndexerBackgroundService>();
                services.Configure<HostOptions>(options => options.ShutdownTimeout = TimeSpan.FromSeconds(30));
            });
}
