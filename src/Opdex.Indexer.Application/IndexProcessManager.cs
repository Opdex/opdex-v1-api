using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Core.Application.Abstractions.Models;
using Opdex.Core.Application.Abstractions.Queries;
using Opdex.Indexer.Application.Abstractions.Queries.Cirrus;
using Opdex.Indexer.Application.Abstractions.Queries.Cirrus.Events;

namespace Opdex.Indexer.Application
{
    public class IndexProcessManager
    {
        private readonly IMediator _mediator;
        private readonly ILogger<IndexProcessManager> _logger;
        
        public IndexProcessManager(IMediator mediator, ILogger<IndexProcessManager> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Todo: Evaluate the usage and necessity of a cancellation token
        // The sync process will be idempotent and there _shouldn't_ be any case of 
        // stopping the sync process unless the app is shut down mid process for an unexpected reason.
        // Using cancellationToken maybe not a good idea in the case that some events from a transaction
        // are persisted and others are not persisted. Since the entire transaction insertion will not be a 
        // SQL "transaction", there will be no rollbacks if a transaction is partially synced.
        // Instead, the lastSyncedBlock would never be updated and the process would attempt to resync
        // the entire block range and all pairs, tokens, and transactions in it.
        public async Task ProcessAsync(CancellationToken cancellationToken)
        {
            try
            {
                // Get current block from Cirrus
                var currentBlock = await _mediator.Send(new RetrieveCirrusCurrentBlockQuery(), cancellationToken);
                
                // Get the last synced block
                var lastSyncedBlock = await _mediator.Send(new RetrieveLastSyncedBlockQuery(), cancellationToken);
                
                if (currentBlock.Height > lastSyncedBlock.Height)
                {
                    // Get latest Pair events from Cirrus
                    var pairEvents = await _mediator.Send(new RetrieveCirrusPairEventsQuery(lastSyncedBlock.Height, currentBlock.Height, "RouterContract"), cancellationToken);

                    foreach (var pairEvent in pairEvents)
                    {
                        // Index each pair
                        // Index each token
                    }

                    // Get all local Opdex Pairs
                    var pairs = await _mediator.Send(new RetrieveAllPairsWithFilterQuery(), cancellationToken);

                    foreach (var pair in pairs)
                    {
                        var pairTransactions = await BuildUniqueTransactions(lastSyncedBlock.Height, currentBlock.Height, pair, cancellationToken);

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

        // Todo: Evaluate these events at the contract level down, do they need to be different events or the same event with a Type flag?
        // If they are separate events on the Pair contracts, should these all be different queries, or one query (maybe with an assembler)
        // that retrieves, groups, and formats all events for each individual transaction.
        private async Task<IDictionary<string, object>> BuildUniqueTransactions(ulong lastSync, ulong currentHeight, PairDto pair, CancellationToken cancellationToken)
        {
            // Get Mint Events
            var mintEvents = await _mediator.Send(new RetrieveCirrusMintEventsByPairQuery(lastSync, currentHeight, "pair.Address"), cancellationToken);

            // Get Burn Events
            var burnEvents = await _mediator.Send(new RetrieveCirrusBurnEventsByPairQuery(lastSync, currentHeight, "pair.Address"), cancellationToken);
                        
            // Get Swap Events
            var swapEvents = await _mediator.Send(new RetrieveCirrusSwapEventsByPairQuery(lastSync, currentHeight, "pair.Address"), cancellationToken);

            // Group By Transaction (Multiple events can occur per transaction)
            // In each events response, it will include the transaction details and ALL event logs. So query each event type log
            // then select each distinct tx Id to a dictionary.

            // Todo: Need to standardize a transaction type with a List<IEvent>
            return new Dictionary<string, object>();
        }
    }
}