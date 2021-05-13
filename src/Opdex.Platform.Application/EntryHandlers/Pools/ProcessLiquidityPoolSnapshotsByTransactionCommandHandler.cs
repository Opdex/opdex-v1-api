using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Pools;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Pools;
using Opdex.Platform.Application.Abstractions.EntryQueries.Pools;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Pools;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.EntryHandlers.Pools
{
    // Todo: Should these store USD values or should that be calculated at the time of the request?
    // When retrieve latest pool snapshots, it should also calculate against current CRS prices
    public class ProcessLiquidityPoolSnapshotsByTransactionCommandHandler : IRequestHandler<ProcessLiquidityPoolSnapshotsByTransactionCommand, Unit>
    {
        private readonly IMediator _mediator;

        public ProcessLiquidityPoolSnapshotsByTransactionCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }


        // Todo: Consider when pulling an SRC token snapshot that may be stale because the pool is low volume
        // If the reserves don't change often but CRS token price still does, token snapshots still need to 
        // occur regularly 
        public async Task<Unit> Handle(ProcessLiquidityPoolSnapshotsByTransactionCommand request, CancellationToken cancellationToken)
        {
            var tx = await _mediator.Send(new RetrieveTransactionByHashQuery(request.TxHash), CancellationToken.None);
            var block = await _mediator.Send(new RetrieveBlockByHeightQuery(tx.BlockHeight), CancellationToken.None);
            var crsToken = await _mediator.Send(new RetrieveTokenByAddressQuery(TokenConstants.Cirrus.Address), CancellationToken.None);
            
            // Handle this better, should technically be RetrieveTokenSnapshotsByTokenIdAndTime or RetrieveTokenSnapshotsByTokenIdWithFilter
            // Should always return a value, either the value for the exact query, the previous snapshot from the requested time, 
            // or a brand new snapshot if none exist. 
            // Maybe GetOrCreateTokenSnapshotsByTokenIdQuery
            var crsSnapshots = await _mediator.Send(new RetrieveActiveTokenSnapshotsByTokenIdQuery(crsToken.Id, block.MedianTime), CancellationToken.None);
            if (!crsSnapshots.Any())
            {
                throw new Exception("CRS snapshots cannot be an empty collection.");
            }

            // Daily or hourly, doesn't matter
            var crsSnapshot = crsSnapshots.FirstOrDefault();
            
            var poolSnapshotGroups = tx.GroupedLogsOfTypes(request.PoolSnapshotLogTypes);
            
            // Each pool in the dictionary
            foreach (var (poolContract, value) in poolSnapshotGroups)
            {
                var pool = await _mediator.Send(new GetLiquidityPoolByAddressQuery(poolContract), CancellationToken.None);
                
                // Todo: Maybe GetOrCreate special case. Retrieve by date, or retrieve record prior to date, or brand new snapshot.
                var poolSnapshots = await _mediator.Send(new RetrieveActiveLiquidityPoolSnapshotsByPoolIdQuery(pool.Id, block.MedianTime), CancellationToken.None);

                // Each snapshot type we care about for pools
                foreach (var snapshotType in request.SnapshotTypes)
                {
                    var snapshotStart = block.MedianTime.ToStartOf(snapshotType);
                    var snapshotEnd = block.MedianTime.ToEndOf(snapshotType);
                    
                    // Adjust query to get snapshots and use Single()
                    var poolSnapshot = poolSnapshots.SingleOrDefault(s => s.SnapshotType == snapshotType) ??
                                       new LiquidityPoolSnapshot(pool.Id, snapshotType, snapshotStart, snapshotEnd);

                    // Each log to process
                    foreach (var poolLog in value)
                    {
                        switch (poolLog.LogType)
                        {
                            case TransactionLogType.ReservesLog:
                                var reservesLog = (ReservesLog)poolLog;
                                await ProcessTokenSnapshot(pool, snapshotType, reservesLog, snapshotStart, snapshotEnd, crsSnapshot);
                                poolSnapshot.ProcessReservesLog(reservesLog, crsSnapshot);
                                break;
                            case TransactionLogType.SwapLog:
                                poolSnapshot.ProcessSwapLog((SwapLog)poolLog, crsSnapshot);
                                break;
                            case TransactionLogType.StartStakingLog:
                                // Todo: Should be odx snapshot
                                poolSnapshot.ProcessStakingLog((StartStakingLog)poolLog, crsSnapshot);
                                break;
                            case TransactionLogType.StopStakingLog:
                                // Todo: Should be odx snapshot
                                poolSnapshot.ProcessStakingLog((StopStakingLog)poolLog, crsSnapshot);
                                break;
                        }
                    }
                    
                    poolSnapshot.IncrementTransactionCount();
                    
                    await _mediator.Send(new MakeLiquidityPoolSnapshotCommand(poolSnapshot), CancellationToken.None);
                }
            }
            
            return Unit.Value;
        }

        private async Task ProcessTokenSnapshot(LiquidityPoolDto pool, SnapshotType snapshotType, ReservesLog log, DateTime snapshotStart, DateTime snapshotEnd,
            TokenSnapshot crsSnapshot)
        {
            var poolTokenSnapshots = await _mediator.Send(new RetrieveActiveTokenSnapshotsByTokenIdQuery(pool.Token.Id, snapshotStart), CancellationToken.None);

            var poolTokenSnapshot = poolTokenSnapshots.SingleOrDefault(s => s.SnapshotType == snapshotType) ??
                                    new TokenSnapshot(pool.Token.Id, 0m, snapshotType, snapshotStart, snapshotEnd);
                    
            poolTokenSnapshot.ProcessReservesLog(log, crsSnapshot, pool.Token.Decimals);

            await _mediator.Send(new MakeTokenSnapshotCommand(poolTokenSnapshot), CancellationToken.None);
        }
    }
}