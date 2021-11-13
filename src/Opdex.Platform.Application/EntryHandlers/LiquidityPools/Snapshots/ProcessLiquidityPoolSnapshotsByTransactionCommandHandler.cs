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

            // Try get CRS snapshot for USD pricing
            var crsSnapshotQuery = new RetrieveTokenSnapshotWithFilterQuery(crsToken.Id, default, block.MedianTime, SnapshotType.Minute);
            var crsSnapshot = await _mediator.Send(crsSnapshotQuery);

            // If it doesn't exist or is stale, refresh
            if (crsSnapshot.Id == 0 || crsSnapshot.EndDate < block.MedianTime)
            {
                await _mediator.Send(new CreateCrsTokenSnapshotsCommand(block.MedianTime, block.Height));
                crsSnapshot = await _mediator.Send(crsSnapshotQuery);
            }

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

                    var lpToken = await _mediator.Send(new RetrieveTokenByAddressQuery(createPoolLog.Pool));
                    tokens[lpToken.Id] = lpToken;

                    await Task.WhenAll(request.SnapshotTypes.Select(async snapshotType =>
                    {
                        var snapshot = new LiquidityPoolSnapshot(liquidityPool.Id, snapshotType, block.MedianTime);

                        await _mediator.Send(new ProcessSrcTokenSnapshotCommand(liquidityPool.MarketId, srcToken, snapshotType, block.MedianTime,
                                                                                crsSnapshot.Price.Close, default, UInt256.Zero, block.Height));

                        await _mediator.Send(new ProcessLpTokenSnapshotCommand(liquidityPool.MarketId, lpToken, snapshot.Reserves.Usd, snapshotType,
                                                                               block.MedianTime, block.Height));

                        await _mediator.Send(new MakeLiquidityPoolSnapshotCommand(snapshot, block.Height));
                    }));
                }));
            }

            // Get the pool snapshot type logs and reserves logs separately so we can always process reserves last
            // Processing reserves last ensures that volume and other costs are associated with the reserves as
            // they were prior to the transaction. V1 contracts log reserve changes first so processing in
            // direct sort order here would incorrectly affect prices associated with the transaction.
            var poolLogs = request.Transaction.LogsOfTypes(request.PoolTransactionSnapshotTypes).ToList();
            var reservesLogs = request.Transaction.LogsOfType<ReservesLog>(TransactionLogType.ReservesLog);
            poolLogs.AddRange(reservesLogs);

            // Each qualifying log that is from a liquidity pool
            foreach (var log in poolLogs)
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

                        stakingTokenSnapshot.ResetStaleSnapshot(crsPerSrc, crsSnapshot.Price.Close, block.MedianTime);
                    }

                    stakingTokenUsd = stakingTokenSnapshot.Price.Close;
                    stakingTokensUsd.TryAdd(market.StakingTokenId, stakingTokenUsd);
                }

                // Each snapshot type for liquidity pools (hourly and daily)
                // This should never be done with Task.WhenAll as it could conflict with the reading/writing of liquidityPoolSnapshots
                foreach(var snapshotType in request.SnapshotTypes)
                {
                    // Try to get any previously adjusted snapshots from other logs without querying the database
                    liquidityPoolSnapshots.TryGetValue(liquidityPool.Id, out var foundSnapshots);

                    var snapshot = foundSnapshots?.FirstOrDefault(snapshot => snapshot.SnapshotType == snapshotType) ??
                                   await _mediator.Send(new RetrieveLiquidityPoolSnapshotWithFilterQuery(liquidityPool.Id, block.MedianTime, snapshotType));

                    // Update a stale snapshot if it is older than what was requested
                    if (snapshot.EndDate < block.MedianTime)
                    {
                        var srcUsd = await _mediator.Send(new ProcessSrcTokenSnapshotCommand(liquidityPool.MarketId, srcToken, snapshotType, snapshot.EndDate,
                                                                                             crsSnapshot.Price.Close, snapshot.Reserves.Crs, snapshot.Reserves.Src,
                                                                                             block.Height));

                        snapshot.ResetStaleSnapshot(crsSnapshot.Price.Close, srcUsd, stakingTokenUsd, srcToken.Sats, block.MedianTime);
                    }

                    if (log.LogType == TransactionLogType.ReservesLog)
                    {
                        var reservesLog = (ReservesLog)log;
                        var srcUsd = await _mediator.Send(new ProcessSrcTokenSnapshotCommand(liquidityPool.MarketId, srcToken, snapshotType,
                                                                                             block.MedianTime, crsSnapshot.Price.Close, reservesLog.ReserveCrs,
                                                                                             reservesLog.ReserveSrc, block.Height));

                        snapshot.ProcessReservesLog(reservesLog, crsSnapshot.Price.Close, srcUsd, srcToken.Sats);

                        await _mediator.Send(new ProcessLpTokenSnapshotCommand(liquidityPool.MarketId, lpToken, snapshot.Reserves.Usd, snapshotType,
                                                                               block.MedianTime, block.Height));
                    }
                    else if (log.LogType == TransactionLogType.SwapLog)
                    {
                        var srcSnapshot = await _mediator.Send(new RetrieveTokenSnapshotWithFilterQuery(srcToken.Id, market.Id, block.MedianTime, SnapshotType.Hourly));

                        snapshot.ProcessSwapLog((SwapLog)log, crsSnapshot.Price.Close, srcSnapshot.Price.Close, srcToken.Sats,  market.IsStakingMarket,
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

            // Persist all snapshot updates at once
            await Task.WhenAll(liquidityPoolSnapshots.Values
                                   .SelectMany(liquidityPool => liquidityPool)
                                   .Select(async snapshot =>
                                   {
                                       snapshot.IncrementTransactionCount();
                                       return await _mediator.Send(new MakeLiquidityPoolSnapshotCommand(snapshot, block.Height));
                                   }));

            return Unit.Value;
        }
    }
}
