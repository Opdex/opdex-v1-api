using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.Http.TextFormatters;

namespace Opdex.Platform.WebApi
{
    public class Program
    {
        private const string LogsDiskDirectory = "logs";
        private static readonly int GeneralFlatFileMaxSizeBytes = 10 * 1024 * 1024; // 10 MB
        private static readonly int DevelopmentFlatFileMaxSizeBytes = 50 * 1024 * 1024; // 50 MB
        public static string AppNameForEnvironment { get; private set; }
        public static bool EnableSwagger { get; private set; }
        
        public static void Main(string[] args)
        {
            // This is just the static global logger used for the host process before DI registrations and container setup has occurred.
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.RollingFile(
                    new NormalTextFormatter(),
                    $@"{LogsDiskDirectory}/global.log",
                    retainedFileCountLimit: 30,
                    fileSizeLimitBytes: GeneralFlatFileMaxSizeBytes,
                    flushToDiskInterval:
                    TimeSpan.FromSeconds(30))
                .CreateLogger();
            
            try
            {
                Log.Information("Starting PLatform Api");
                var host = CreateHostBuilder(args).Build();    
                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "PLatform Api application start-up failed");
            }

            Log.CloseAndFlush();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .UseSerilog((context, loggingConfiguration) =>
                {
                    // get OPDEX__ENVIRONMENT name
                    // var opdexDeploymentEnvironment = context.Configuration["OPDEX:ENVIRONMENT"] ?? throw new Exception("OPDEX__ENVIRONMENT deployment environment variable must be provided.");
                    var opdexDeploymentEnvironment = "DEV";
                    
                    // Enable Swagger
                    // EnableSwagger = bool.TryParse(context.Configuration["OPDEX:ENABLESWAGGER"], out var result) && result;

                    // get hosting environment and log name
                    var environment = context.HostingEnvironment;

                    // Log.Information($"Using opdex environment:{opdexDeploymentEnvironment} and dotnet environment:{environment.EnvironmentName}");

                    // Qualified Application Name
                    environment.ApplicationName = "Opdex-PLatformApi";

                    // used for logging
                    AppNameForEnvironment = $"{opdexDeploymentEnvironment.ToUpperInvariant()}-{environment.ApplicationName}";

                    // Create local file based log path for development environment
                    var rootLogFolder = $@"{environment.ContentRootPath}/{LogsDiskDirectory}";
                    
                    if (context.HostingEnvironment.IsProduction())
                    {
                        // Todo: Point logging towards service
                    }
                    else
                    {
                        loggingConfiguration
                            .WriteTo.Console()
                            .WriteTo.File(new NormalTextFormatter(),
                                $@"{rootLogFolder}/webapi.log",
                                rollOnFileSizeLimit: true,
                                fileSizeLimitBytes: DevelopmentFlatFileMaxSizeBytes,
                                retainedFileCountLimit: 5);
                    }
                });
    }
}