using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Opdex.Indexer.WebApi
{
    /// <summary>
    /// Hosted background service responsible for kicking off indexer process
    /// every few seconds.
    /// </summary>
    public class IndexerBackgroundService : BackgroundService
    {
        private bool _indexActive;

        private readonly ILogger<IndexerBackgroundService> _logger;
        private readonly IServiceProvider _services;

        public IndexerBackgroundService(IServiceProvider services, ILogger<IndexerBackgroundService> logger)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (_indexActive)
            {
                await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
                _logger.LogInformation("Would ping Cirrus FN and kick off sync.");
                // Runs every few (2-3) seconds
                // Initiates Process via IndexProcessManager
            }
        }
        
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _indexActive = false;
            return base.StopAsync(cancellationToken);
        }
        
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _indexActive = true;
            return base.StartAsync(cancellationToken);
        }
    }
}