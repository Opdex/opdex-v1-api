using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Pools;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Pools;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Application.Abstractions.Queries.Pools.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Snapshots;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Tokens;
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
            var block = await _mediator.Send(new RetrieveBlockByHeightQuery(request.Transaction.BlockHeight, findOrThrow: true));
            var blockTime = block.MedianTime;

            var crsToken = await _mediator.Send(new RetrieveTokenByAddressQuery(TokenConstants.Cirrus.Address, findOrThrow: true));

            var crsSnapshotQuery = new RetrieveTokenSnapshotWithFilterQuery(crsToken.Id, 0, blockTime, SnapshotType.Minute);
            var crsSnapshot = await _mediator.Send(crsSnapshotQuery);

            if (crsSnapshot.Price.Close <= 0 || crsSnapshot.EndDate < blockTime)
            {
                await _mediator.Send(new CreateCrsTokenSnapshotsCommand(blockTime));
                crsSnapshot = await _mediator.Send(crsSnapshotQuery);
            }

            // Transaction logs grouped by liquidity pool
            var poolSnapshotGroups = request.Transaction.GroupedLogsOfTypes(request.PoolSnapshotLogTypes);

            // Each pool in the dictionary
            foreach (var (poolContract, transactionLogs) in poolSnapshotGroups)
            {
                var liquidityPool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(poolContract, findOrThrow: true));
                var market = await _mediator.Send(new RetrieveMarketByIdQuery(liquidityPool.MarketId, findOrThrow: true));
                var lpToken = await _mediator.Send(new RetrieveTokenByIdQuery(liquidityPool.LpTokenId, findOrThrow: true));
                var stakingTokenUsd = 0m;

                if (market.IsStakingMarket)
                {
                    var stakingTokenId = market.StakingTokenId.GetValueOrDefault();
                    var stakingTokenSnapshotQuery = new RetrieveTokenSnapshotWithFilterQuery(stakingTokenId, liquidityPool.MarketId, blockTime, SnapshotType.Hourly);
                    var stakingTokenSnapshot = await _mediator.Send(stakingTokenSnapshotQuery);

                    // Update a stale snapshot if it is older than what was requested
                    if (stakingTokenSnapshot.EndDate < blockTime)
                    {
                        var stakingTokenPool = await _mediator.Send(new RetrieveLiquidityPoolBySrcTokenIdAndMarketIdQuery(stakingTokenId, market.Id, findOrThrow: true));
                        var stakingTokenPoolSnapshot = await _mediator.Send(new RetrieveLiquidityPoolSnapshotWithFilterQuery(stakingTokenPool.Id,
                            stakingTokenSnapshot.EndDate, SnapshotType.Hourly));

                        var crsPerSrc = stakingTokenPoolSnapshot.Cost.CrsPerSrc.Close;

                        stakingTokenSnapshot.ResetStaleSnapshot(crsPerSrc, crsSnapshot.Price.Close, blockTime);
                    }

                    stakingTokenUsd = stakingTokenSnapshot.Price.Close;
                }

                var srcToken = await _mediator.Send(new RetrieveTokenByIdQuery(liquidityPool.SrcTokenId, findOrThrow: true));

                // Each snapshot type we care about for pools (hour & day)
                foreach (var snapshotType in request.SnapshotTypes)
                {
                    // Retrieves the snapshot in range, the most recent one prior, or a newly instantiated snapshot
                    var liquidityPoolSnapshot = await _mediator.Send(new RetrieveLiquidityPoolSnapshotWithFilterQuery(liquidityPool.Id, blockTime, snapshotType));

                    // Update a stale snapshot if it is older than what was requested
                    if (liquidityPoolSnapshot.EndDate < blockTime)
                    {
                        var srcUsd = await ProcessSrcTokenSnapshot(liquidityPool.MarketId, srcToken, snapshotType, liquidityPoolSnapshot.EndDate, crsSnapshot.Price.Close,
                            ulong.Parse(liquidityPoolSnapshot.Reserves.Crs), liquidityPoolSnapshot.Reserves.Src);

                        liquidityPoolSnapshot.ResetStaleSnapshot(crsSnapshot.Price.Close, srcUsd, stakingTokenUsd, srcToken.Decimals, blockTime);
                    }

                    // Each log to process
                    //
                    // Important to understand/consider order of logs that are being processed.
                    // Should always be processed in the order they occured in contract with _some_ exceptions.
                    // Some situations, such as tracking volume, consider scenarios where a ReservesLog is output before a SwapLog resulting
                    // in the volume USD amounts being calculated according to updated pricing from *after* the swap occured. To correctly find the
                    // transaction/snapshots volume USD, it should be computed based on current reserves/pricing *prior* to the swap transaction.
                    // Todo: Fix volume calculation based on the comment above
                    foreach (var poolLog in transactionLogs)
                    {
                        switch (poolLog.LogType)
                        {
                            case TransactionLogType.ReservesLog:
                                var reservesLog = (ReservesLog)poolLog;

                                // Process SRC Token Snapshot
                                var srcUsd = await ProcessSrcTokenSnapshot(liquidityPool.MarketId, srcToken, snapshotType, blockTime, crsSnapshot.Price.Close,
                                    reservesLog.ReserveCrs, reservesLog.ReserveSrc);

                                // Process LP Snapshot
                                liquidityPoolSnapshot.ProcessReservesLog(reservesLog, crsSnapshot.Price.Close, srcUsd, srcToken.Decimals);

                                // Prepare LP Token Snapshot
                                var totalSupplyDecimal = lpToken.TotalSupply.ToRoundedDecimal(lpToken.Decimals, lpToken.Decimals);
                                var lptUsd = liquidityPoolSnapshot.Reserves.Usd / totalSupplyDecimal;

                                // Process LP Token Snapshot
                                await ProcessLpTokenSnapshot(liquidityPool.MarketId, lpToken, snapshotType, blockTime, lptUsd);

                                break;
                            case TransactionLogType.SwapLog:
                                liquidityPoolSnapshot.ProcessSwapLog((SwapLog)poolLog, crsSnapshot.Price.Close, market.IsStakingMarket, market.TransactionFee, market.MarketFeeEnabled);
                                break;
                            case TransactionLogType.StakeLog:
                                liquidityPoolSnapshot.ProcessStakingLog((StakeLog)poolLog, stakingTokenUsd);
                                break;
                        }
                    }

                    // Todo: Consider persisting which TxHashes have been included in the snapshot
                    // Storing TxHashes could be redundant amongst different snapshot types, consider performance and storage options
                    // Daily snapshots could get heavy.
                    // Alternative option is to Process LP Snapshots by block where we can also process Market Snapshots with valid Txs
                    // Used for when a single smart contract transaction may hop between two pools. Those two pools will store 1 transaction
                    // each but when processing market snapshots, would result in 1 + 1 incorrectly.
                    liquidityPoolSnapshot.IncrementTransactionCount();

                    await _mediator.Send(new MakeLiquidityPoolSnapshotCommand(liquidityPoolSnapshot));
                }
            }

            return Unit.Value;
        }

        private async Task<decimal> ProcessSrcTokenSnapshot(long marketId, Token token, SnapshotType snapshotType, DateTime blockTime, decimal crsUsd,
            ulong reserveCrs, string reserveSrc)
        {
            var tokenSnapshotQuery = new RetrieveTokenSnapshotWithFilterQuery(token.Id, marketId, blockTime, snapshotType);
            var tokenSnapshot = await _mediator.Send(tokenSnapshotQuery);

            // Update a stale snapshot if it is older than what was requested
            if (tokenSnapshot.EndDate < blockTime)
            {
                var crsPerSrc = reserveCrs.Token0PerToken1(reserveSrc, token.Sats);
                tokenSnapshot.ResetStaleSnapshot(crsPerSrc, crsUsd, blockTime);
            }
            else
            {
                tokenSnapshot.UpdatePrice(reserveCrs, reserveSrc, crsUsd, token.Sats);
            }

            await _mediator.Send(new MakeTokenSnapshotCommand(tokenSnapshot));

            return tokenSnapshot.Price.Close;
        }

        private async Task ProcessLpTokenSnapshot(long marketId, Token token, SnapshotType snapshotType, DateTime blockTime, decimal lptUsd)
        {
            var tokenSnapshotQuery = new RetrieveTokenSnapshotWithFilterQuery(token.Id, marketId, blockTime, snapshotType);
            var tokenSnapshot = await _mediator.Send(tokenSnapshotQuery);

            // Update a stale snapshot if it is older than what was requested
            if (tokenSnapshot.EndDate < blockTime)
            {
                tokenSnapshot.ResetStaleSnapshot(lptUsd, blockTime);
            }
            else
            {
                tokenSnapshot.UpdatePrice(lptUsd);
            }

            await _mediator.Send(new MakeTokenSnapshotCommand(tokenSnapshot));
        }
    }
}