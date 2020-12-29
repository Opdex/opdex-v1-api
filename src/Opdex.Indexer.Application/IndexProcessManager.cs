using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Core.Application.Abstractions.Queries;
using Opdex.Core.Application.Abstractions.Queries.Cirrus;
using Opdex.Core.Infrastructure.Abstractions.Integrations.CirrusFullNodeApi;

namespace Opdex.Indexer.Application
{
    public class IndexProcessManager
    {
        private readonly IMediator _mediator;
        private readonly ICirrusClient _cirrusClient;
        private readonly ILogger<IndexProcessManager> _logger;
        
        public IndexProcessManager(IMediator mediator, ICirrusClient cirrusClient, ILogger<IndexProcessManager> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _cirrusClient = cirrusClient ?? throw new ArgumentNullException(nameof(cirrusClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task ProcessAsync()
        {
            try
            {
                // 1. CFN RetrieveCirrusCurrentBlockQuery
                var currentBlock = await _mediator.Send(new RetrieveCirrusCurrentBlockQuery());
                
                // 2. RetrieveLastSyncedBlockQuery
                var lastSyncedBlock = await _mediator.Send(new RetrieveLastSyncedBlockQuery());
                
                // Todo: register queries/handlers/response types and remove casting
                if (currentBlock.Height > lastSyncedBlock.Height)
                {
                    // 1. RetrieveAllPairsWithFilter
                    var pairs = await _mediator.Send(new RetrieveAllPairsWithFilterQuery());
                    
                    // 2. RetrieveCirrusPairEvents
                    var pairEvents = await _mediator.Send(new RetrieveCirrusPairEventsQuery());
                    
                    // 3. Filter/Index Any New Pairs
                    //     - MakePairCommand -> PersistPairCommand
                    
                    // 4. ForEach DB Pair
                    //     i. Index Mint Events
                    //         - Index transaction and event if not exists
                    //     i. Index Burn Events
                    //         - Index transaction and event if not exists
                    //     i. Index Swap Events
                    //         - Index transaction and event if not exists
                    //     i. UpdateLiquidity
                }
            }
            catch (Exception ex)
            {
                // Log and swallow all exceptions
                _logger.LogError(ex, "Failed to sync");
            }
        }
    }
}