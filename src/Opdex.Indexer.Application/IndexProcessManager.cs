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
using Opdex.Indexer.Domain.Models;
using Opdex.Indexer.Domain.Models.LogEvents;

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
                            var tx = await _mediator.Send(new RetrieveCirrusTransactionByHashQuery(txHash), cancellationToken);
                            var isToRouter = tx.To == "RouterContract";
                            var distinctPairsWithTransactions = tx.Events
                                .Where(l => pairs.TryGetValue(l.Address, out _))
                                .Select(l => l.Address)
                                .Distinct()
                                .ToList();

                            // Definitely a more performant way of doing this using a dictionary
                            pairsWithTransactions
                                .AddRange(distinctPairsWithTransactions
                                    .Where(d => !pairsWithTransactions.Contains(d)));
                            
                            if (!isToRouter && !distinctPairsWithTransactions.Any())
                            {
                                continue;
                            }
                            
                            await _mediator.Send(new MakeTransactionCommand(tx), cancellationToken);
                        }
                        
                        // Update block sync status to complete

                        queuedBlock = queuedBlock.NexBlockHash.HasValue()
                            ? await _mediator.Send(new RetrieveCirrusBlockByHashQuery(queuedBlock.NexBlockHash), cancellationToken)
                            : null;
                    } 
                    while (queuedBlock != null && currentBlock.Height >= queuedBlock.Height);

                    // Any pairs with transactions, get latest reserves, updating pricing, etc.
                    // Todo: This loop may not be necessary
                    foreach (var pair in pairsWithTransactions)
                    {
                        
                    }
                }
            }
            catch (Exception ex)
            {
                // Log and swallow all exceptions
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
            // Get latest Pair events from Cirrus
            var pairEvents = await _mediator.Send(new RetrieveCirrusPairEventsQuery(latestHeight, currentHeight, "RouterContract"), cancellationToken);

            // Create new pairs and tokens
            foreach (var pairEvent in pairEvents)
            {
                // Todo: Error handling / logging
                await _mediator.Send(new MakeTokenCommand(pairEvent.Token), cancellationToken);
                await _mediator.Send(new MakePairCommand(pairEvent.Pair), cancellationToken);
            }
            
            // Get latest list of all pairs
            var allPairs = await _mediator.Send(new RetrieveAllPairsWithFilterQuery(), cancellationToken);
            
            return allPairs.ToDictionary(p => p.Address);
        }
    }
}