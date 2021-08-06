using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Pools;
using Opdex.Platform.Application.Abstractions.EntryCommands.Pools;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Application.Abstractions.Queries.Pools.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Snapshots;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;

namespace Opdex.Platform.Application.EntryHandlers.Pools.Snapshots
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
            var blockTime = block.MedianTime;

            var crsToken = await _mediator.Send(new RetrieveTokenByAddressQuery(TokenConstants.Cirrus.Address));

            var crsSnapshotQuery = new RetrieveTokenSnapshotWithFilterQuery(crsToken.Id, 0, blockTime, SnapshotType.Minute);
            var crsSnapshot = await _mediator.Send(crsSnapshotQuery);

            if (crsSnapshot.Id == 0 || crsSnapshot.EndDate < blockTime)
            {
                await _mediator.Send(new CreateCrsTokenSnapshotsCommand(blockTime));
                crsSnapshot = await _mediator.Send(crsSnapshotQuery);
            }

            var crsUsd = crsSnapshot.Price.Close;

            // Transaction logs grouped by liquidity pool
            var poolSnapshotGroups = request.Transaction.GroupedLogsOfTypes(request.PoolSnapshotLogTypes);

            // Each pool in the dictionary
            foreach (var (poolContract, transactionLogs) in poolSnapshotGroups)
            {
                var liquidityPool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(poolContract));
                var market = await _mediator.Send(new RetrieveMarketByIdQuery(liquidityPool.MarketId));
                var lpToken = await _mediator.Send(new RetrieveTokenByIdQuery(liquidityPool.LpTokenId));
                var stakingTokenUsd = 0m;

                if (market.IsStakingMarket)
                {
                    var stakingTokenId = market.StakingTokenId.GetValueOrDefault();

                    var stakingTokenSnapshot = await _mediator.Send(new RetrieveTokenSnapshotWithFilterQuery(stakingTokenId,
                                                                                                             liquidityPool.MarketId,
                                                                                                             blockTime,
                                                                                                             SnapshotType.Hourly));

                    // Update a stale snapshot if it is older than what was requested
                    if (stakingTokenSnapshot.EndDate < blockTime)
                    {
                        var stakingTokenPool = await _mediator.Send(new RetrieveLiquidityPoolBySrcTokenIdAndMarketIdQuery(stakingTokenId,
                                                                                                                          market.Id));

                        var stakingTokenPoolSnapshot = await _mediator.Send(new RetrieveLiquidityPoolSnapshotWithFilterQuery(stakingTokenPool.Id,
                                                                                                                             stakingTokenSnapshot.EndDate,
                                                                                                                             SnapshotType.Hourly));

                        var crsPerSrc = stakingTokenPoolSnapshot.Cost.CrsPerSrc.Close;

                        stakingTokenSnapshot.ResetStaleSnapshot(crsPerSrc, crsUsd, blockTime);
                    }

                    stakingTokenUsd = stakingTokenSnapshot.Price.Close;
                }

                var srcToken = await _mediator.Send(new RetrieveTokenByIdQuery(liquidityPool.SrcTokenId));

                // Each snapshot type we care about for pools (hour & day)
                foreach (var snapshotType in request.SnapshotTypes)
                {
                    // Retrieves the snapshot in range, the most recent one prior, or a newly instantiated snapshot
                    var liquidityPoolSnapshot = await _mediator.Send(new RetrieveLiquidityPoolSnapshotWithFilterQuery(liquidityPool.Id,
                                                                                                                      blockTime,
                                                                                                                      snapshotType));
                    // Update a stale snapshot if it is older than what was requested
                    if (liquidityPoolSnapshot.EndDate < blockTime)
                    {
                        var srcUsd = await _mediator.Send(new ProcessSrcTokenSnapshotCommand(liquidityPool.MarketId,
                                                                                             srcToken,
                                                                                             snapshotType,
                                                                                             liquidityPoolSnapshot.EndDate,
                                                                                             crsUsd,
                                                                                             liquidityPoolSnapshot.Reserves.Crs,
                                                                                             liquidityPoolSnapshot.Reserves.Src));

                        liquidityPoolSnapshot.ResetStaleSnapshot(crsUsd, srcUsd, stakingTokenUsd, srcToken.Sats, blockTime);
                    }

                    // Todo: Consider prioritizing the order to Swap, Stake, Reserves
                    // Currently, reserves logs first, then we update USD prices and calc volume. Volume probably should be based on
                    // USD prices prior to the reserves change.
                    foreach (var poolLog in transactionLogs)
                    {
                        if (poolLog.LogType == TransactionLogType.ReservesLog)
                        {
                            var reservesLog = (ReservesLog)poolLog;

                            // Process SRC Token Snapshot
                            var srcUsd = await _mediator.Send(new ProcessSrcTokenSnapshotCommand(liquidityPool.MarketId,
                                                                                                 srcToken,
                                                                                                 snapshotType,
                                                                                                 blockTime,
                                                                                                 crsUsd,
                                                                                                 reservesLog.ReserveCrs,
                                                                                                 reservesLog.ReserveSrc));

                            // Process Reserves and Token Pricing Snapshot
                            liquidityPoolSnapshot.ProcessReservesLog(reservesLog, crsUsd, srcUsd, srcToken.Sats);

                            // Process LP Token Snapshot
                            await _mediator.Send(new ProcessLpTokenSnapshotCommand(liquidityPool.MarketId,
                                                                                   lpToken,
                                                                                   liquidityPoolSnapshot.Reserves.Usd,
                                                                                   snapshotType,
                                                                                   blockTime));
                        }
                        else if (poolLog.LogType == TransactionLogType.SwapLog)
                        {
                            var srcSnapshot = await _mediator.Send(new RetrieveTokenSnapshotWithFilterQuery(srcToken.Id,
                                                                                                            market.Id,
                                                                                                            blockTime,
                                                                                                            SnapshotType.Hourly));

                            // Process Volume of a Swap
                            liquidityPoolSnapshot.ProcessSwapLog((SwapLog)poolLog, crsUsd, srcSnapshot.Price.Close, srcToken.Sats,
                                                                 market.IsStakingMarket, market.TransactionFee, market.MarketFeeEnabled);
                        }
                        else if (poolLog.LogType == TransactionLogType.StartStakingLog ||
                                 poolLog.LogType == TransactionLogType.StopStakingLog)
                        {
                            // Process Staking Weight
                            liquidityPoolSnapshot.ProcessStakingLog((StakeLog)poolLog, stakingTokenUsd);
                        }
                    }

                    // Todo: Consider persisting which TxHashes or another identifier of which transactions have been included.
                    liquidityPoolSnapshot.IncrementTransactionCount();

                    await _mediator.Send(new MakeLiquidityPoolSnapshotCommand(liquidityPoolSnapshot));
                }
            }

            return Unit.Value;
        }
    }
}
