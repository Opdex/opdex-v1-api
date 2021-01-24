using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Opdex.Core.Application.Abstractions.Models;
using Opdex.Core.Application.Abstractions.Queries;
using Opdex.Core.Common;
using Opdex.Core.Common.Extensions;
using Opdex.Core.Domain.Models.TransactionReceipt;
using Opdex.Indexer.Application.Abstractions;
using Opdex.Indexer.Application.Abstractions.Commands;
using Opdex.Indexer.Application.Abstractions.Queries.Cirrus;
using Opdex.Indexer.Application.Abstractions.Queries.Cirrus.Events;

namespace Opdex.Indexer.Application
{
    public class IndexProcessManager : IIndexProcessManager
    {
        private readonly OpdexConfiguration _opdexConfiguration;
        private readonly IMediator _mediator;
        private readonly ILogger<IndexProcessManager> _logger;
        
        public IndexProcessManager(IOptions<OpdexConfiguration> opdexConfiguration, IMediator mediator, ILogger<IndexProcessManager> logger)
        {
            _opdexConfiguration = opdexConfiguration.Value ?? throw new ArgumentNullException(nameof(opdexConfiguration));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task ProcessAsync(CancellationToken cancellationToken)
        {
            var latestBlock = await _mediator.Send(new RetrieveLatestBlockQuery(), cancellationToken);
            var latestBlockReceipt = await _mediator.Send(new RetrieveCirrusBlockByHashQuery(latestBlock.Hash), cancellationToken);

            if (latestBlockReceipt?.NextBlockHash?.HasValue() != true)
            {
                return;
            }
            
            var pairs = await ProcessNewPairs(latestBlock.Height, cancellationToken);
                
            var queuedBlock = await _mediator.Send(new RetrieveCirrusBlockByHashQuery(latestBlockReceipt.NextBlockHash), cancellationToken);
            
            while (queuedBlock != null && !cancellationToken.IsCancellationRequested)
            {
                var pairsEngagedInQueuedBlock = new List<string>();
                
                // Todo: Fetch and persist $ CRS/STRAX price for this block
                // if (block.time isAround dateTime.UtcNow) Persist Latest Strax/Cirrus $ CMC Price
                // else Persist Historical Strax/Cirrus $ CMC Price and Index
                
                await _mediator.Send(new MakeBlockCommand(), cancellationToken);
                
                foreach (var txHash in queuedBlock.Tx)
                {
                    var transaction = await ProcessTransaction(txHash, pairs, cancellationToken);
                    if (transaction == null)
                    {
                        continue;
                    }
                    
                    var pairsEngagedInTransaction = transaction.PairsEngaged
                        .Where(d => !pairsEngagedInQueuedBlock.Contains(d));
                    
                    pairsEngagedInQueuedBlock.AddRange(pairsEngagedInTransaction);
                }
                
                foreach (var pairAddress in pairsEngagedInQueuedBlock)
                {
                    var foundPair = pairs.TryGetValue(pairAddress, out var pair);
                    if (!foundPair)
                    {
                        _logger.LogError($"Did find pair {pairAddress} to update");
                        continue;
                    }
                    
                    // Update general info for the block (e.g. SyncEvent would update reserves/pricing. Swap event would update fees)
                    // Per block, per pair, maybe per token, with reserves, fees, and costs (sats & usd)
                    // Todo: Consider doing this when transactions and events are processed depending on how heavy the workload is
                }
                
                // Todo: Update block sync status to complete
                // In the process of this, should transactions w/ failures be persisted or just logged?
                // Persisting the transaction hash of unexpected failures might be beneficial for back-fills / resync
                // if and when this issue ever occurs

                queuedBlock = queuedBlock.NextBlockHash.HasValue()
                    ? await _mediator.Send(new RetrieveCirrusBlockByHashQuery(queuedBlock.NextBlockHash), cancellationToken)
                    : null;
            }
        }

        /// <summary>
        /// Fetches and persists new tokens and pairs from opdex router contract event logs.
        /// </summary>
        /// <param name="latestHeight">Opdex synced latest block height</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>Dictionary of pair address keys with pair values</returns>
        private async Task<IDictionary<string, PairDto>> ProcessNewPairs(ulong latestHeight, CancellationToken cancellationToken)
        {
            var pairEvents = await _mediator.Send(new RetrieveCirrusPairEventsQuery(latestHeight), cancellationToken);

            foreach (var pairEvent in pairEvents)
            {
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
        /// <param name="cancellationToken">cancellation token: should probably be removed from indexer methods</param>
        private async Task<TransactionReceipt> ProcessTransaction(string txHash, IDictionary<string, PairDto> pairs, CancellationToken cancellationToken)
        {
            var tx = await _mediator.Send(new RetrieveCirrusTransactionByHashQuery(txHash), cancellationToken);
            var isToRouter = tx.To == _opdexConfiguration.ControllerContract;
            var pairsEngaged = tx.Events
                .Where(l => pairs.TryGetValue(l.Address, out _))
                .Select(l => l.Address)
                .Distinct()
                .ToList();

            tx.SetPairsEngaged(pairsEngaged);
                            
            if (!isToRouter && !pairsEngaged.Any())
            {
                return null;
            }
                            
            await _mediator.Send(new MakeTransactionCommand(tx), cancellationToken);

            return tx;
        }
    }
}