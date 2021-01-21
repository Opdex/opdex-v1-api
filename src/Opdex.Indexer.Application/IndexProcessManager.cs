using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Core.Application.Abstractions.Models;
using Opdex.Core.Application.Abstractions.Queries;
using Opdex.Core.Common.Extensions;
using Opdex.Indexer.Application.Abstractions;
using Opdex.Indexer.Application.Abstractions.Commands;
using Opdex.Indexer.Application.Abstractions.Queries.Cirrus;
using Opdex.Indexer.Application.Abstractions.Queries.Cirrus.Events;

namespace Opdex.Indexer.Application
{
    public class IndexProcessManager : IIndexProcessManager
    {
        private readonly IMediator _mediator;
        private readonly ILogger<IndexProcessManager> _logger;
        
        public IndexProcessManager(IMediator mediator, ILogger<IndexProcessManager> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task ProcessAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing Block");
            
            try
            {
                var currentBlock = await _mediator.Send(new RetrieveCirrusCurrentBlockQuery(), cancellationToken);
                var latestBlock = await _mediator.Send(new RetrieveLatestBlockQuery(), cancellationToken);
                
                if (currentBlock.Height > latestBlock.Height)
                {
                    var pairs = await ProcessNewPairs(latestBlock.Height, currentBlock.Height, cancellationToken);
                    var pairsWithTransactions = new List<string>();
                    
                    // Todo: This really should be getting the latestBlock.Height, use that to query cirrus for latestBlock + 1;
                    var queuedBlock = await _mediator.Send(new RetrieveCirrusBlockByHashQuery(latestBlock.Hash), cancellationToken);
                    
                    // Loop each block from last sync to current
                    do
                    {
                        // if (block.time isAround dateTime.UtcNow) Persist Latest Strax/Cirrus $ CMC Price
                        // else Persist Historical Strax/Cirrus $ CMC Price and Index 
                        await _mediator.Send(new MakeBlockCommand(), cancellationToken);
                        
                        // Loop each transaction in the block
                        foreach (var txHash in queuedBlock.Tx)
                        {
                            await ProcessTransaction(txHash, pairs, pairsWithTransactions, cancellationToken);
                        }
                        
                        // Update block sync status to complete

                        queuedBlock = queuedBlock.NexBlockHash.HasValue()
                            ? await _mediator.Send(new RetrieveCirrusBlockByHashQuery(queuedBlock.NexBlockHash), cancellationToken)
                            : null;
                    } 
                    while (queuedBlock != null && currentBlock.Height >= queuedBlock.Height);

                    // Any pairs with transactions, get latest reserves, updating pricing, etc.
                    // Todo: This loop may not be necessary
                    // Possibly run this during each block being synced if there are relevant transaction events
                    // (e.g. SyncEvent would update reserves/pricing. Swap event would update fees)
                    // Persist this data in a new table "pair_history" where a record is inserted 
                    // per block, per pair, with reserves, fees, and costs (sats & usd)
                    foreach (var pair in pairsWithTransactions)
                    {
                        
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to sync");
            }
        }

        /// <summary>
        /// Fetches and persists new tokens and pairs from opdex router contract event logs.
        /// </summary>
        /// <param name="latestHeight">Opdex synced latest block height</param>
        /// <param name="currentHeight">Cirrus chain actual current height</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>Dictionary of pair address keys with pair values</returns>
        private async Task<IDictionary<string, PairDto>> ProcessNewPairs(ulong latestHeight, ulong currentHeight, CancellationToken cancellationToken)
        {
            var pairEvents = await _mediator.Send(new RetrieveCirrusPairEventsQuery(latestHeight, currentHeight, "RouterContract"), cancellationToken);

            foreach (var pairEvent in pairEvents)
            {
                // Todo: Error handling / logging
                await _mediator.Send(new MakeTokenCommand(pairEvent.Token), cancellationToken);
                await _mediator.Send(new MakePairCommand(pairEvent.Pair), cancellationToken);
            }
            
            var allPairs = await _mediator.Send(new RetrieveAllPairsWithFilterQuery(), cancellationToken);
            
            return allPairs.ToDictionary(p => p.Address);
        }

        /// <summary>
        /// Fetches and processes an Opdex transaction
        /// </summary>
        /// <param name="txHash">Transaction hash to process</param>
        /// <param name="pairs">Collection of all known Opdex pairs</param>
        /// <param name="pairsWithTransactions">Distinct list of pairs called in this transaction</param>
        /// <param name="cancellationToken">cancellation token: should probably be removed from indexer methods</param>
        private async Task ProcessTransaction(string txHash, IDictionary<string, PairDto> pairs, List<string> pairsWithTransactions, CancellationToken cancellationToken)
        {
            var tx = await _mediator.Send(new RetrieveCirrusTransactionByHashQuery(txHash), cancellationToken);
            var isToRouter = tx.To == "RouterContract";
            var distinctPairsWithTransactions = tx.Events
                .Where(l => pairs.TryGetValue(l.Address, out _))
                .Select(l => l.Address)
                .Distinct()
                .ToList();

            // Definitely a more performant way of doing this using a dictionary
            // Todo: Refactor this approach in general - ugly
            pairsWithTransactions
                .AddRange(distinctPairsWithTransactions
                    .Where(d => !pairsWithTransactions.Contains(d)));
                            
            if (!isToRouter && !distinctPairsWithTransactions.Any())
            {
                return;
            }
                            
            await _mediator.Send(new MakeTransactionCommand(tx), cancellationToken);
        }
    }
}