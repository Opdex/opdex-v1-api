using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Opdex.Core.Application.Abstractions.Models;
using Opdex.Core.Application.Abstractions.Queries.Blocks;
using Opdex.Core.Common;
using Opdex.Core.Common.Extensions;
using Opdex.Core.Domain.Models;
using Opdex.Indexer.Application.Abstractions;
using Opdex.Indexer.Application.Abstractions.Commands.Blocks;
using Opdex.Indexer.Application.Abstractions.Commands.Transactions;
using Opdex.Indexer.Application.Abstractions.Queries.Blocks;
using Opdex.Indexer.Application.Abstractions.Queries.Transactions;

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
            
            var pools = await ProcessNewPools(latestBlock.Height, cancellationToken);
                
            var queuedBlock = await _mediator.Send(new RetrieveCirrusBlockByHashQuery(latestBlockReceipt.NextBlockHash), cancellationToken);
            
            while (queuedBlock != null && !cancellationToken.IsCancellationRequested)
            {
                var poolsEngagedInQueuedBlock = new List<string>();
                
                // Todo: Fetch and persist $ CRS/STRAX price for this block
                // if (block.time isAround dateTime.UtcNow) Persist Latest Strax/Cirrus $ CMC Price
                // else Persist Historical Strax/Cirrus $ CMC Price and Index
                
                // await _mediator.Send(new MakeBlockCommand(), cancellationToken);
                
                foreach (var txHash in queuedBlock.Tx)
                {
                    var transaction = await ProcessTransaction(txHash, pools, cancellationToken);
                    if (transaction == null)
                    {
                        continue;
                    }
                    
                    var poolsEngagedInTransaction = transaction.PoolsEngaged
                        .Where(d => !poolsEngagedInQueuedBlock.Contains(d));
                    
                    poolsEngagedInQueuedBlock.AddRange(poolsEngagedInTransaction);
                }
                
                foreach (var poolAddress in poolsEngagedInQueuedBlock)
                {
                    var foundPool = pools.TryGetValue(poolAddress, out var pool);
                    if (!foundPool)
                    {
                        _logger.LogError($"Did find pool {poolAddress} to update");
                        continue;
                    }
                    
                    // Update general info for the block (e.g. ReservesLog would update reserves/pricing. Swap log would update fees)
                    // Per block, per pool, maybe per token, with reserves, fees, and costs (sats & usd)
                    // Todo: Consider doing this when transactions and logs are processed depending on how heavy the workload is
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
        /// Fetches and persists new tokens and pools from opdex router contract log logs.
        /// </summary>
        /// <param name="latestHeight">Opdex synced latest block height</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>Dictionary of pool address keys with pool values</returns>
        private Task<Dictionary<string, PoolDto>> ProcessNewPools(ulong latestHeight, CancellationToken cancellationToken)
        {
            // var poolLogs = await _mediator.Send(new RetrieveCirrusPoolLogsQuery(latestHeight), cancellationToken);
            //
            // foreach (var poolLog in poolLogs)
            // {
            //     var tokenId = await _mediator.Send(new MakeTokenCommand(poolLog.Token), cancellationToken);
            //     await _mediator.Send(new MakePoolCommand(poolLog.Pool, tokenId), cancellationToken);
            // }
            //
            // var allPools = await _mediator.Send(new RetrieveAllPoolsWithFilterQuery(), cancellationToken);
            //
            // return allPools.ToDictionary(p => p.Address);

            var dictionary = new Dictionary<string, PoolDto>();
            return Task.FromResult(dictionary);
        }

        /// <summary>
        /// Fetches and processes an Opdex transaction
        /// </summary>
        /// <param name="txHash">Transaction hash to process</param>
        /// <param name="pools">Collection of all known Opdex pools</param>
        /// <param name="cancellationToken">cancellation token: should probably be removed from indexer methods</param>
        private async Task<Transaction> ProcessTransaction(string txHash, IDictionary<string, PoolDto> pools, CancellationToken cancellationToken)
        {
            var tx = await _mediator.Send(new RetrieveCirrusTransactionByHashQuery(txHash), cancellationToken);
            var isToRouter = tx.To == _opdexConfiguration.ControllerContract;
            var poolsEngaged = tx.Logs
                .Where(l => pools.TryGetValue(l.Contract, out _))
                .Select(l => l.Contract)
                .Distinct()
                .ToList();

            tx.SetPoolsEngaged(poolsEngaged);
                            
            if (!isToRouter && !poolsEngaged.Any())
            {
                return null;
            }
                            
            await _mediator.Send(new MakeTransactionCommand(tx), cancellationToken);

            return tx;
        }
    }
}