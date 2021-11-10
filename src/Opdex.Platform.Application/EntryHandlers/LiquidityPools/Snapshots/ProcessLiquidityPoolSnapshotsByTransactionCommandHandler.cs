using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.LiquidityPools;
using Opdex.Platform.Application.Abstractions.EntryCommands.LiquidityPools;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Snapshots;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using Opdex.Platform.Domain.Models.TransactionLogs.Markets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.LiquidityPools.Snapshots
{
    public class ProcessLiquidityPoolSnapshotsByTransactionCommandHandler : IRequestHandler<ProcessLiquidityPoolSnapshotsByTransactionCommand, Unit>
    {
        private readonly IMediator _mediator;

        public ProcessLiquidityPoolSnapshotsByTransactionCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<Unit> Handle(ProcessLiquidityPoolSnapshotsByTransactionCommand request, CancellationToken cancellationToken)
        {
            var block = await _mediator.Send(new RetrieveBlockByHeightQuery(request.Transaction.BlockHeight));
            var crsToken = await _mediator.Send(new RetrieveTokenByAddressQuery(Address.Cirrus));
            var crsSummary = await _mediator.Send(new RetrieveTokenSummaryByMarketAndTokenIdQuery(default, crsToken.Id));

            // Create dictionaries in memory to reduce redundant calls to the db
            var markets = new Dictionary<ulong, Market>();
            var liquidityPools = new Dictionary<Address, LiquidityPool>();
            var liquidityPoolSnapshots = new Dictionary<ulong, List<LiquidityPoolSnapshot>>();
            var tokens = new Dictionary<ulong, Token>();
            var stakingTokensUsd = new Dictionary<ulong, decimal>();

            // Create initial snapshots for new liquidity pools and their associated SRC token
            var createPoolLogs = request.Transaction.LogsOfType<CreateLiquidityPoolLog>(TransactionLogType.CreateLiquidityPoolLog);
            if (createPoolLogs.Any())
            {
                await Task.WhenAll(createPoolLogs.Select(async createPoolLog =>
                {
                    var liquidityPool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(createPoolLog.Pool));
                    liquidityPools[liquidityPool.Address] = liquidityPool;

                    var srcToken = await _mediator.Send(new RetrieveTokenByAddressQuery(createPoolLog.Token));
                    tokens[srcToken.Id] = srcToken;

                    await Task.WhenAll(request.SnapshotTypes.Select(async snapshotType =>
                    {
                        var snapshot = new LiquidityPoolSnapshot(liquidityPool.Id, snapshotType, block.MedianTime);

                        await _mediator.Send(new ProcessSrcTokenSnapshotCommand(liquidityPool.MarketId, srcToken, snapshotType, block.MedianTime,
                                                                                crsSummary.PriceUsd, default, UInt256.Zero, block.Height));

                        await _mediator.Send(new MakeLiquidityPoolSnapshotCommand(snapshot, block.Height));
                    }));
                }));
            }

            // Each qualifying log that is from a liquidity pool
            // Todo: Swap logs are logged after updated reserves logs, the order of those processed logs should be reversed.
            // Processing swaps first gives more accurate USD totals of the swap prior to the reserves change however, we should technically
            // update potentially stale *current* reserves which would update the pool's SRC USD price, then process swap transaction w/ volume
            // and rewards, then process the new reserve totals
            foreach (var log in request.Transaction.LogsOfTypes(request.PoolSnapshotLogTypes))
            {
                var foundLiquidityPool = liquidityPools.TryGetValue(log.Contract, out var liquidityPool);
                if (!foundLiquidityPool)
                {
                    liquidityPool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(log.Contract));
                    liquidityPools[liquidityPool.Address] = liquidityPool;
                }

                var foundMarket = markets.TryGetValue(liquidityPool.MarketId, out var market);
                if (!foundMarket)
                {
                    market = await _mediator.Send(new RetrieveMarketByIdQuery(liquidityPool.MarketId));
                    markets[market.Id] = market;
                }

                var foundLpToken = tokens.TryGetValue(liquidityPool.LpTokenId, out var lpToken);
                if (!foundLpToken)
                {
                    lpToken = await _mediator.Send(new RetrieveTokenByIdQuery(liquidityPool.LpTokenId));
                    tokens[lpToken.Id] = lpToken;
                }

                var foundSrcToken = tokens.TryGetValue(liquidityPool.SrcTokenId, out var srcToken);
                if (!foundSrcToken)
                {
                    srcToken = await _mediator.Send(new RetrieveTokenByIdQuery(liquidityPool.SrcTokenId));
                    tokens[srcToken.Id] = srcToken;
                }

                // Lookup staking token pricing info used for liquidity pool staking totals
                var foundStakingToken = !stakingTokensUsd.TryGetValue(market.StakingTokenId, out var stakingTokenUsd);
                if (market.IsStakingMarket && !foundStakingToken)
                {
                    var stakingTokenSnapshot = await _mediator.Send(new RetrieveTokenSnapshotWithFilterQuery(market.StakingTokenId, liquidityPool.MarketId,
                                                                                                             block.MedianTime, SnapshotType.Hourly));

                    // Update a stale snapshot if it is older than what was requested
                    if (stakingTokenSnapshot.EndDate < block.MedianTime)
                    {
                        var stakingTokenPool = await _mediator.Send(new RetrieveLiquidityPoolBySrcTokenIdAndMarketIdQuery(market.StakingTokenId, market.Id));

                        var stakingTokenPoolSnapshot = await _mediator.Send(new RetrieveLiquidityPoolSnapshotWithFilterQuery(stakingTokenPool.Id,
                                                                                                                             stakingTokenSnapshot.EndDate,
                                                                                                                             SnapshotType.Hourly));

                        var crsPerSrc = stakingTokenPoolSnapshot.Cost.CrsPerSrc.Close;

                        stakingTokenSnapshot.ResetStaleSnapshot(crsPerSrc, crsSummary.PriceUsd, block.MedianTime);
                    }

                    stakingTokenUsd = stakingTokenSnapshot.Price.Close;
                    stakingTokensUsd.TryAdd(market.StakingTokenId, stakingTokenUsd);
                }

                // Try to get any previously adjusted snapshots from other logs without querying the database
                liquidityPoolSnapshots.TryGetValue(liquidityPool.Id, out var foundSnapshots);

                // Each snapshot type for liquidity pools (hourly and daily)
                foreach(var snapshotType in request.SnapshotTypes)
                {
                    var snapshot = foundSnapshots?.FirstOrDefault(snapshot => snapshot.SnapshotType == snapshotType) ??
                                   await _mediator.Send(new RetrieveLiquidityPoolSnapshotWithFilterQuery(liquidityPool.Id, block.MedianTime, snapshotType));

                    // Update a stale snapshot if it is older than what was requested
                    if (snapshot.EndDate < block.MedianTime)
                    {
                        var srcUsd = await _mediator.Send(new ProcessSrcTokenSnapshotCommand(liquidityPool.MarketId, srcToken, snapshotType, snapshot.EndDate,
                                                                                             crsSummary.PriceUsd, snapshot.Reserves.Crs, snapshot.Reserves.Src,
                                                                                             block.Height));

                        snapshot.ResetStaleSnapshot(crsSummary.PriceUsd, srcUsd, stakingTokenUsd, srcToken.Sats, block.MedianTime);
                    }

                    if (log.LogType == TransactionLogType.ReservesLog)
                    {
                        var reservesLog = (ReservesLog)log;
                        var srcUsd = await _mediator.Send(new ProcessSrcTokenSnapshotCommand(liquidityPool.MarketId, srcToken, snapshotType,
                                                                                             block.MedianTime, crsSummary.PriceUsd, reservesLog.ReserveCrs,
                                                                                             reservesLog.ReserveSrc, block.Height));

                        snapshot.ProcessReservesLog(reservesLog, crsSummary.PriceUsd, srcUsd, srcToken.Sats);

                        await _mediator.Send(new ProcessLpTokenSnapshotCommand(liquidityPool.MarketId, lpToken, snapshot.Reserves.Usd, snapshotType,
                                                                               block.MedianTime, block.Height));
                    }
                    else if (log.LogType == TransactionLogType.SwapLog)
                    {
                        var srcSnapshot = await _mediator.Send(new RetrieveTokenSnapshotWithFilterQuery(srcToken.Id, market.Id, block.MedianTime, SnapshotType.Hourly));

                        snapshot.ProcessSwapLog((SwapLog)log, crsSummary.PriceUsd, srcSnapshot.Price.Close, srcToken.Sats,  market.IsStakingMarket,
                                                market.TransactionFee, market.MarketFeeEnabled);
                    }
                    else if (log.LogType == TransactionLogType.StartStakingLog || log.LogType == TransactionLogType.StopStakingLog)
                    {
                        snapshot.ProcessStakingLog((StakeLog)log, stakingTokenUsd);
                    }

                    // Get the "other" snapshot from the dictionary, there can only be hourly or daily, so other is whatever isn't this current loops snapshot type
                    var otherSnapshot = foundSnapshots?.FirstOrDefault(foundSnapshot => foundSnapshot.SnapshotType != snapshotType);

                    // Order doesn't matter here for persistence
                    var updatedSnapshots = new List<LiquidityPoolSnapshot> { snapshot };
                    if (otherSnapshot != null) updatedSnapshots.Add(otherSnapshot);

                    // Set the value of the liquidity pool snapshots
                    liquidityPoolSnapshots[liquidityPool.Id] = updatedSnapshots;
                }
            }

            // Todo: Bug somewhere in here, this code is persisting liquidty pool summaries and token snapshots just fine w/ swap transactions
            // pool_liquidity_snapshots are not persisting and no errors are being thrown
            //
            // Persist all snapshot updates at once
            await Task.WhenAll(liquidityPoolSnapshots.Values.Select(async poolSnapshots =>
            {
                await Task.WhenAll(poolSnapshots.Select(async snapshot =>
                {
                    snapshot.IncrementTransactionCount();
                    await _mediator.Send(new MakeLiquidityPoolSnapshotCommand(snapshot, block.Height));
                }));
            }));

            return Unit.Value;
        }
    }
}
