using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Core.Application.Abstractions.Queries;
using Opdex.Indexer.Application.Abstractions.Queries.Cirrus;
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
                // Get current block from Cirrus
                var currentBlock = await _mediator.Send(new RetrieveCirrusCurrentBlockQuery());
                
                // Get the last synced block
                var lastSyncedBlock = await _mediator.Send(new RetrieveLastSyncedBlockQuery());
                
                if (currentBlock.Height > lastSyncedBlock.Height)
                {
                    // Get latest Pair events from Cirrus
                    var pairEvents = await _mediator.Send(new RetrieveCirrusPairEventsQuery(lastSyncedBlock.Height, currentBlock.Height));

                    foreach (var pairEvent in pairEvents)
                    {
                        // Index each pair
                        // Index each token
                    }

                    // Get all local Opdex Pairs
                    var pairs = await _mediator.Send(new RetrieveAllPairsWithFilterQuery());

                    // Persist transactions per pair
                    foreach (var pair in pairs)
                    {
                        var pairTransactions = await BuildUniqueTransactions(lastSyncedBlock.Height, currentBlock.Height);

                        foreach (var tx in pairTransactions.ToList())
                        {
                            // Index Transaction
                        }

                        // Update Pair Liquidity
                    }
                    
                    // Persist Current Block
                }
            }
            catch (Exception ex)
            {
                // Log and swallow all exceptions
                _logger.LogError(ex, "Failed to sync");
            }
        }

        private async Task<IDictionary<string, object>> BuildUniqueTransactions(ulong lastSync, ulong currentHeight)
        {
            // Get Mint Events
            var mintEvents = await _mediator.Send(new RetrieveCirrusMintEventsByPairQuery(lastSync, currentHeight));

            // Get Burn Events
            var burnEvents = await _mediator.Send(new RetrieveCirrusBurnEventsByPairQuery(lastSync, currentHeight));
                        
            // Get Swap Events
            var swapEvents = await _mediator.Send(new RetrieveCirrusSwapEventsByPairQuery(lastSync, currentHeight));

            // Group By Transaction (Multiple events can occur per transaction)
            // In each events response, it will include the transaction details and ALL event logs. So query each event type log
            // then select each distinct tx Id to a dictionary.

            // Todo: Need to standardize a transaction type with a List<IEvent>
            return new Dictionary<string, object>();
        }
    }
}