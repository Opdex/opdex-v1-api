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
using Opdex.Platform.Domain.Models.Pools;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;

namespace Opdex.Platform.Application.EntryHandlers.Pools
{
    // Todo: Process LP Token Snapshot somewhere in here
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

            if (crsSnapshot.Price <= 0 || crsSnapshot.EndDate < blockTime)
            {
                await _mediator.Send(new CreateCrsTokenSnapshotsCommand(blockTime));
                crsSnapshot = await _mediator.Send(crsSnapshotQuery) ?? throw new Exception("Error finding CRS snapshot.");
            }

            // Transaction logs grouped by liquidity pool
            var poolSnapshotGroups = request.Transaction.GroupedLogsOfTypes(request.PoolSnapshotLogTypes);

            // Each pool in the dictionary
            foreach (var (poolContract, transactionLogs) in poolSnapshotGroups)
            {
                var liquidityPool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(poolContract, findOrThrow: true));
                var market = await _mediator.Send(new RetrieveMarketByIdQuery(liquidityPool.MarketId, findOrThrow: true));
                var stakingTokenId = market.StakingTokenId ?? 0;

                TokenSnapshot stakingTokenSnapshot = null;

                if (stakingTokenId > 0)
                {
                    var stakingTokenSnapshotQuery = new RetrieveTokenSnapshotWithFilterQuery(stakingTokenId, liquidityPool.MarketId, blockTime, SnapshotType.Hourly);
                    stakingTokenSnapshot = await _mediator.Send(stakingTokenSnapshotQuery);

                    // TODO: Check for stale snapshot
                }

                // Each snapshot type we care about for pools
                foreach (var snapshotType in request.SnapshotTypes)
                {
                    // Retrieves the snapshot in range or the most recent one prior
                    var snapshot = await _mediator.Send(new RetrieveLiquidityPoolSnapshotWithFilterQuery(liquidityPool.Id, blockTime, snapshotType));

                    // Update stale snapshots if they are older than what was requested
                    if (snapshot.EndDate < blockTime)
                    {
                        // Reset snapshot USD amounts and roll over reserve amounts to new snapshot
                        snapshot.ResetStaleSnapshot(crsSnapshot.Price, stakingTokenSnapshot?.Price ?? 0, blockTime);
                    }

                    // Each log to process
                    foreach (var poolLog in transactionLogs)
                    {
                        switch (poolLog.LogType)
                        {
                            case TransactionLogType.ReservesLog:
                                var reservesLog = (ReservesLog)poolLog;
                                var token = await _mediator.Send(new RetrieveTokenByIdQuery(liquidityPool.SrcTokenId, findOrThrow: true));
                                await ProcessTokenSnapshot(liquidityPool, token, snapshotType, reservesLog, blockTime, crsSnapshot.Price);
                                snapshot.ProcessReservesLog(reservesLog, crsSnapshot.Price, token.Sats);
                                break;
                            case TransactionLogType.SwapLog:
                                snapshot.ProcessSwapLog((SwapLog)poolLog, crsSnapshot.Price, stakingTokenId > 0, market.TransactionFee, market.MarketFeeEnabled);
                                break;
                            case TransactionLogType.StakeLog:
                                snapshot.ProcessStakingLog((StakeLog)poolLog, stakingTokenSnapshot?.Price ?? 0);
                                break;
                        }
                    }

                    snapshot.IncrementTransactionCount();

                    await _mediator.Send(new MakeLiquidityPoolSnapshotCommand(snapshot));
                }
            }

            return Unit.Value;
        }

        private async Task ProcessTokenSnapshot(LiquidityPool pool, Token token, SnapshotType snapshotType, ReservesLog log, DateTime blockTime, decimal crsUsd)
        {
            var tokenSnapshotQuery = new RetrieveTokenSnapshotWithFilterQuery(pool.SrcTokenId, pool.MarketId, blockTime, snapshotType);
            var tokenSnapshot = await _mediator.Send(tokenSnapshotQuery);

            if (tokenSnapshot.EndDate < blockTime)
            {
                // Todo: Update stale snapshot
            }

            tokenSnapshot.ProcessReservesLog(log, crsUsd, token.Sats);

            await _mediator.Send(new MakeTokenSnapshotCommand(tokenSnapshot));
        }
    }
}