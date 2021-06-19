using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Pools;
using Opdex.Platform.Application.Abstractions.EntryCommands;
using Opdex.Platform.Application.Abstractions.EntryCommands.Blocks;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens.Snapshots;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryQueries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Application.Abstractions.Queries.Pools.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Snapshots;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Application.EntryHandlers.Blocks
{
    public class ProcessLatestBlocksCommandHandler : IRequestHandler<ProcessLatestBlocksCommand, Unit>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProcessLatestBlocksCommandHandler> _logger;

        public ProcessLatestBlocksCommandHandler(IMediator mediator, ILogger<ProcessLatestBlocksCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Todo: Forks and Chain Reorgs :(
        public async Task<Unit> Handle(ProcessLatestBlocksCommand request, CancellationToken cancellationToken)
        {
            // The latest synced block we have, if none, the tip of cirrus chain
            var previousBlock = await _mediator.Send(new GetBestBlockQuery(), cancellationToken);

            // Process each block until we reach the chain tip
            while (previousBlock?.NextBlockHash != null && !cancellationToken.IsCancellationRequested)
            {
                // Retrieve and create the block
                var currentBlock = await _mediator.Send(new RetrieveCirrusBlockByHashQuery(previousBlock.NextBlockHash), CancellationToken.None);
                var blockCreated = await _mediator.Send(new CreateBlockCommand(currentBlock), CancellationToken.None);

                // Stop if the block wasn't created
                if (!blockCreated)
                {
                    break;
                }

                var isNewYear = previousBlock.MedianTime.Year < currentBlock.MedianTime.Year;
                var isNewMonth = isNewYear || previousBlock.MedianTime.Month < currentBlock.MedianTime.Month;
                var isNewDay = isNewMonth || previousBlock.MedianTime.Day < currentBlock.MedianTime.Day;
                var isNewMinute = isNewDay || previousBlock.MedianTime.Minute < currentBlock.MedianTime.Minute;

                if (isNewMinute)
                {
                    // Dev Environment = 15 minutes, otherwise 1 minute
                    if (!request.IsDevelopEnv || currentBlock.MedianTime.Minute == 15)
                    {
                        await _mediator.Send(new CreateCrsTokenSnapshotsCommand(currentBlock.MedianTime), CancellationToken.None);
                    }
                }

                var crs = await _mediator.Send(new RetrieveTokenByAddressQuery(TokenConstants.Cirrus.Address));

                var crsSnapshot = await _mediator.Send(new RetrieveTokenSnapshotWithFilterQuery(crs.Id, 0, currentBlock.MedianTime, SnapshotType.Minute));

                if (isNewDay)
                {
                    await ProcessDailySnapshotRefresh(currentBlock.MedianTime, crsSnapshot.Price.Close);
                }

                await ProcessBlockTransactions(currentBlock);

                // Consideration: process liquidity pool snapshots after each block rather than during each transaction.

                ProcessMarketSnapshots();

                previousBlock = currentBlock;
            }

            return Unit.Value;
        }

        /// <summary>
        /// Using daily liquidity pool snapshots, process the latest market snapshot
        /// </summary>
        private static void ProcessMarketSnapshots()
        {
            var markets = new List<Market>();

            foreach (var market in markets)
            {
                // Get all daily market snapshots
                // Get all daily liquidity pool snapshots in market
                // Process market snapshot from lp snapshots
            }
        }

        /// <summary>
        /// First block of the day, create all new daily snapshots for each token, pool, and market
        /// Note: This is not necessarily built to scale... May need to be revisited in the future.
        /// </summary>
        /// <param name="blockTime">Current block time</param>
        /// <param name="crsUsd">Current CRS USd price</param>
        private async Task ProcessDailySnapshotRefresh(DateTime blockTime, decimal crsUsd)
        {
            blockTime = blockTime.ToStartOf(SnapshotType.Daily);
            var snapshotTypes = new List<SnapshotType> {SnapshotType.Hourly, SnapshotType.Daily};

            // Todo: Get all markets
            var markets = new List<Market>();

            var tokenList = await _mediator.Send(new RetrieveAllTokensQuery());
            var tokens = tokenList.ToDictionary(k => k.Id);

            foreach (var market in markets)
            {
                // Todo: Modify query to require a MarketId
                var marketPools = await _mediator.Send(new RetrieveAllPoolsQuery());
                var stakingTokenUsd = 0m;

                if (market.IsStakingMarket)
                {
                    if (!tokens.TryGetValue(market.StakingTokenId.GetValueOrDefault(), out var stakingToken))
                    {
                        continue;
                    }

                    var liquidityPool = await _mediator.Send(new RetrieveLiquidityPoolBySrcTokenIdAndMarketIdQuery(stakingToken.Id, market.Id));

                    if (!tokens.TryGetValue(liquidityPool.LpTokenId, out var lpToken))
                    {
                        continue;
                    }

                    foreach (var snapshotType in snapshotTypes)
                    {
                        await ProcessLiquidityPoolSnapshot(liquidityPool.Id, market.Id, stakingToken, lpToken, crsUsd, snapshotType, blockTime);
                    }

                    var stakingTokenSnapshot = await _mediator.Send(new RetrieveTokenSnapshotWithFilterQuery(stakingToken.Id, market.Id, blockTime, SnapshotType.Daily));
                    stakingTokenUsd = stakingTokenSnapshot.Price.Close;
                }

                // Every pool excluding the staking token and it's liquidity pool
                foreach(var liquidityPool in marketPools.Where(mp => mp.SrcTokenId != market.StakingTokenId))
                {
                    if (!tokens.TryGetValue(liquidityPool.SrcTokenId, out var srcToken) ||
                        !tokens.TryGetValue(liquidityPool.LpTokenId, out var lpToken))
                    {
                        continue;
                    }

                    foreach (var snapshotType in snapshotTypes)
                    {
                        await ProcessLiquidityPoolSnapshot(liquidityPool.Id, market.Id, srcToken, lpToken, crsUsd, snapshotType, blockTime, stakingTokenUsd);
                    }
                }

                // Process market snapshot
            }
        }

        private async Task ProcessLiquidityPoolSnapshot(long liquidityPoolId, long marketId, Token srcToken, Token lpToken, decimal crsUsd,
                                                        SnapshotType snapshotType, DateTime blockTime, decimal? stakingTokenUsd = null)
        {
            // Retrieve liquidity pool snapshot
            var lpSnapshot = await _mediator.Send(new RetrieveLiquidityPoolSnapshotWithFilterQuery(liquidityPoolId,
                                                                                                   blockTime,
                                                                                                   snapshotType));
            // Process new token snapshot
            var srcUsd = await _mediator.Send(new ProcessSrcTokenSnapshotCommand(marketId,
                                                                                 srcToken,
                                                                                 snapshotType,
                                                                                 blockTime,
                                                                                 crsUsd,
                                                                                 lpSnapshot.Reserves.Crs,
                                                                                 lpSnapshot.Reserves.Src));

            // When processing a liquidity pool of a staking token, use the srcUsd value instead.
            var stakingUsd = stakingTokenUsd ?? srcUsd;

            // Process latest lp snapshot
            lpSnapshot.ResetStaleSnapshot(crsUsd, srcUsd, stakingUsd, srcToken.Decimals, blockTime);

            await _mediator.Send(new MakeLiquidityPoolSnapshotCommand(lpSnapshot));

            // Process latest lp token snapshot
            var lptUsd = await _mediator.Send(new ProcessLpTokenSnapshotCommand(marketId,
                                                                                lpToken,
                                                                                lpSnapshot.Reserves.Usd,
                                                                                snapshotType,
                                                                                blockTime));
        }

        /// <summary>
        /// Loops and processes each individual transaction in the block
        /// </summary>
        /// <param name="currentBlock">Current block receipt</param>
        private async Task ProcessBlockTransactions(BlockReceipt currentBlock)
        {
            foreach (var tx in currentBlock.TxHashes.Where(tx => tx != currentBlock.MerkleRoot))
            {
                try
                {
                    await _mediator.Send(new CreateTransactionCommand(tx), CancellationToken.None);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Unable to create transaction with error: {ex.Message}");
                }
            }
        }
    }
}