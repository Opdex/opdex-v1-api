using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Opdex.Indexer.Application.Abstractions;

namespace Opdex.Indexer.WebApi
{
    public class IndexerBackgroundService : BackgroundService
    {
        private readonly ILogger<IndexerBackgroundService> _logger;
        private readonly IServiceProvider _services;

        public IndexerBackgroundService(IServiceProvider services, ILogger<IndexerBackgroundService> logger)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var started = false;
            
            while (!cancellationToken.IsCancellationRequested)
            {
                if (!started)
                {
                    await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
                }

                try
                {
                    using var scope = _services.CreateScope();
                    var indexManager = 
                        scope.ServiceProvider
                            .GetRequiredService<IIndexProcessManager>();

                    await indexManager.ProcessAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, "Critical error processing Cirrus blocks");
                }
                finally
                {
                    started = true;
                }
            }
        }
        
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
        
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }
    }
}