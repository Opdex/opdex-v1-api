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
                    
                    var blockIndex = await _mediator.Send(new RetrieveCirrusBlockByHashQuery(latestBlock.Hash), cancellationToken);
                    
                    // Loop each block from last sync to current
                    do
                    {
                        foreach (var txHash in blockIndex.Tx)
                        {
                            var tx = await _mediator.Send(new RetrieveCirrusTransactionByHashQuery(txHash), cancellationToken);
                            var isToRouter = tx.To == "RouterContract";
                            var distinctPairsWithTransactions = tx.Logs
                                .Where(l => pairs.TryGetValue(l.Address, out _))
                                .Select(l => l.Address)
                                .Distinct().ToList();
                            
                            distinctPairsWithTransactions.ForEach(p =>
                            {
                                if (!pairsWithTransactions.Contains(p))
                                {
                                    pairsWithTransactions.Add(p);
                                }
                            });
                            
                            if (isToRouter || distinctPairsWithTransactions.Any())
                            {
                                // parse all logs to relevant events
                                // calculate transaction fees
                                // index events
                            }
                        }
                        
                        // 2. Update Block Details
                        //     - if (block.time isAround dateTime.UtcNow) Get Latest Strax/Cirrus $ CMC Price and Index
                        //     - else Get Historical Strax/Cirrus $ CMC Price and Index
                        //     - Set sync status boolean 
                        
                        
                        blockIndex = blockIndex.NexBlockHash.HasValue()
                            ? await _mediator.Send(new RetrieveCirrusBlockByHashQuery(blockIndex.NexBlockHash), cancellationToken)
                            : null;
                    } 
                    while (blockIndex != null && currentBlock.Height >= blockIndex.Height);

                    // Any pairs with transactions, get latest reserves, updating pricing, etc.
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

        private async Task<IDictionary<string, PairDto>> ProcessNewPairs(ulong latestHeight, ulong currentHeight, CancellationToken cancellationToken)
        {
            // Get latest Pair events from Cirrus
            var pairEvents = await _mediator.Send(new RetrieveCirrusPairEventsQuery(latestHeight, currentHeight, "RouterContract"), cancellationToken);

            // Create new pairs and tokens
            foreach (var pairEvent in pairEvents)
            {
                var token = await _mediator.Send(new MakeTokenCommand(pairEvent.Token), cancellationToken);
                var pair = await _mediator.Send(new MakePairCommand(pairEvent.Pair), cancellationToken);
            }
            
            // Get latest list of all pairs
            var allPairs = await _mediator.Send(new RetrieveAllPairsWithFilterQuery(), cancellationToken);
            
            return allPairs.ToDictionary(p => p.Address);
        }
    }
}