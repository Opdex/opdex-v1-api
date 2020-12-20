using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Opdex.Indexer.WebApi
{
    public class IndexerProcessor : BackgroundService
    {
        private bool _indexActive = false;

        private readonly ILogger<IndexerProcessor> _logger;
        private readonly IServiceProvider _services;

        public IndexerProcessor(IServiceProvider services, ILogger<IndexerProcessor> logger)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (_indexActive)
            {
                //Todo: Initiate Process
                // 1. Ping CFN every few (2-3) seconds
                // 2. Get LastSyncedBlock
                // 3. Process if latest is newer than LastSyncedBlock
                //     a. Index any new pairs and any new tokens
                //     b. Foreach Pair
                //          i. Index Mint Events
                //              - Index transaction and event if not exists
                //          i. Index Burn Events
                //              - Index transaction and event if not exists
                //          i. Index Swap Events
                //              - Index transaction and event if not exists
            }

            return Task.CompletedTask;
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