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
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using System;
using System.Collections.Generic;
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
            var blockTime = block.MedianTime;

            var crsToken = await _mediator.Send(new RetrieveTokenByAddressQuery(Address.Cirrus));

            var crsSnapshotQuery = new RetrieveTokenSnapshotWithFilterQuery(crsToken.Id, 0, blockTime, SnapshotType.Minute);
            var crsSnapshot = await _mediator.Send(crsSnapshotQuery);

            if (crsSnapshot.Id == 0 || crsSnapshot.EndDate < blockTime)
            {
                await _mediator.Send(new CreateCrsTokenSnapshotsCommand(blockTime));
                crsSnapshot = await _mediator.Send(crsSnapshotQuery);
            }

            var crsUsd = crsSnapshot.Price.Close;

            // Transaction logs grouped by liquidity pool
            var logsGroupedByLiquidityPool = request.Transaction.GroupedLogsOfTypes(request.PoolSnapshotLogTypes);

            // Each pool in the dictionary
            foreach ((Address poolContract, List<TransactionLog> transactionLogs) in logsGroupedByLiquidityPool)
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
                    var snapshot = await _mediator.Send(new RetrieveLiquidityPoolSnapshotWithFilterQuery(liquidityPool.Id,
                                                                                                                      blockTime,
                                                                                                                      snapshotType));
                    // Update a stale snapshot if it is older than what was requested
                    if (snapshot.EndDate < blockTime)
                    {
                        var srcUsd = await _mediator.Send(new ProcessSrcTokenSnapshotCommand(liquidityPool.MarketId,
                                                                                             srcToken,
                                                                                             snapshotType,
                                                                                             snapshot.EndDate,
                                                                                             crsUsd,
                                                                                             snapshot.Reserves.Crs,
                                                                                             snapshot.Reserves.Src));

                        snapshot.ResetStaleSnapshot(crsUsd, srcUsd, stakingTokenUsd, srcToken.Sats, blockTime);
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
                            snapshot.ProcessReservesLog(reservesLog, crsUsd, srcUsd, srcToken.Sats);

                            // Process LP Token Snapshot
                            await _mediator.Send(new ProcessLpTokenSnapshotCommand(liquidityPool.MarketId,
                                                                                   lpToken,
                                                                                   snapshot.Reserves.Usd,
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
                            snapshot.ProcessSwapLog((SwapLog)poolLog, crsUsd, srcSnapshot.Price.Close, srcToken.Sats,
                                                                 market.IsStakingMarket, market.TransactionFee, market.MarketFeeEnabled);
                        }
                        else if (poolLog.LogType == TransactionLogType.StartStakingLog || poolLog.LogType == TransactionLogType.StopStakingLog)
                        {
                            // Process Staking Weight
                            snapshot.ProcessStakingLog((StakeLog)poolLog, stakingTokenUsd);
                        }
                    }

                    // Todo: Consider persisting which TxHashes or another identifier of which transactions have been included.
                    snapshot.IncrementTransactionCount();

                    // Only update the summary on the daily snapshot
                    if (snapshotType == SnapshotType.Daily)
                    {
                        var summary = await _mediator.Send(new RetrieveLiquidityPoolSummaryByLiquidityPoolIdQuery(liquidityPool.Id, false));

                        summary ??= new LiquidityPoolSummary(liquidityPool.Id, block.Height);

                        summary.Update(snapshot, block.Height);

                        await _mediator.Send(new MakeLiquidityPoolSummaryCommand(summary));
                    }

                    await _mediator.Send(new MakeLiquidityPoolSnapshotCommand(snapshot));
                }
            }

            return Unit.Value;
        }
    }
}
