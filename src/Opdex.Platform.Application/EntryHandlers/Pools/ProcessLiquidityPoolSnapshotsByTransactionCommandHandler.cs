using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Pools;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Pools;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryQueries.Pools;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Vault;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.Pools;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;

namespace Opdex.Platform.Application.EntryHandlers.Pools
{
    // Todo: Should these store USD values or should that be calculated at the time of the request?
    // When retrieve latest pool snapshots, it should also calculate against current CRS prices
    public class ProcessLiquidityPoolSnapshotsByTransactionCommandHandler : IRequestHandler<ProcessLiquidityPoolSnapshotsByTransactionCommand, Unit>
    {
        private readonly IMediator _mediator;
        private IModelAssembler<Transaction, TransactionDto> _assembler;

        public ProcessLiquidityPoolSnapshotsByTransactionCommandHandler(IMediator mediator, IModelAssembler<Transaction, TransactionDto> assembler)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _assembler = assembler ?? throw new ArgumentNullException(nameof(assembler));
        }


        // Todo: Consider when pulling an SRC token snapshot that may be stale because the pool is low volume
        // If the reserves don't change often but CRS token price still does, token snapshots still need to 
        // occur regularly 
        public async Task<Unit> Handle(ProcessLiquidityPoolSnapshotsByTransactionCommand request, CancellationToken cancellationToken)
        {
            var tx = request.Transaction;
            var block = await _mediator.Send(new RetrieveBlockByHeightQuery(tx.BlockHeight, findOrThrow: true), CancellationToken.None);
            var crsToken = await _mediator.Send(new RetrieveTokenByAddressQuery(TokenConstants.Cirrus.Address, findOrThrow: true), CancellationToken.None);
            
            // Todo: Handle this better...Maybe using a GetOrCreateTokenSnapshotsByTokenIdAndMarketIdAndTimeQuery
            var crsSnapshots = await _mediator.Send(new RetrieveTokenSnapshotsByTokenIdAndMarketIdAndTimeQuery(crsToken.Id, 0, block.MedianTime), CancellationToken.None);
            if (!crsSnapshots.Any())
            {
                await _mediator.Send(new CreateCrsTokenSnapshotsCommand(block.MedianTime), CancellationToken.None);
                crsSnapshots = await _mediator.Send(new RetrieveTokenSnapshotsByTokenIdAndMarketIdAndTimeQuery(crsToken.Id, 0, block.MedianTime), CancellationToken.None);
            }
            
            var vaultQuery = new RetrieveVaultQuery(findOrThrow: true);
            var vault = await _mediator.Send(vaultQuery, CancellationToken.None);

            var odxQuery = new RetrieveTokenByIdQuery(vault.TokenId, findOrThrow: true);
            var odx = await _mediator.Send(odxQuery, CancellationToken.None);

            // Preferably the minute snapshot
            var crsSnapshot = crsSnapshots.OrderBy(s => s.SnapshotType).FirstOrDefault();
            
            // Logs grouped by liquidity pool to snapshot
            var poolSnapshotGroups = tx.GroupedLogsOfTypes(request.PoolSnapshotLogTypes);
            
            // Each pool in the dictionary
            foreach (var (poolContract, value) in poolSnapshotGroups)
            {
                var poolQuery = new GetLiquidityPoolByAddressQuery(poolContract);
                var pool = await _mediator.Send(poolQuery, CancellationToken.None);
                
                // Todo: Handle this better...Maybe using a GetOrCreateTokenSnapshotsByTokenIdAndMarketIdAndTimeQuery
                var odxSnapshots = await _mediator.Send(new RetrieveTokenSnapshotsByTokenIdAndMarketIdAndTimeQuery(odx.Id, pool.Market.Id, block.MedianTime), CancellationToken.None);
                if (!odxSnapshots.Any())
                {
                    // Todo: ODX snapshot
                    // await _mediator.Send(new CreateCrsTokenSnapshotsCommand(block.MedianTime), CancellationToken.None);
                    odxSnapshots = await _mediator.Send(new RetrieveTokenSnapshotsByTokenIdAndMarketIdAndTimeQuery(odx.Id, pool.Market.Id, block.MedianTime), CancellationToken.None);
                } 
                
                // Preferably the minute snapshot
                var odxSnapshot = odxSnapshots.OrderBy(s => s.SnapshotType).FirstOrDefault() ?? 
                                  new TokenSnapshot(odx.Id, pool.Market.Id, 0m, SnapshotType.Minute, 
                                      block.MedianTime.ToStartOf(SnapshotType.Minute), block.MedianTime.ToEndOf(SnapshotType.Minute));
                
                // Todo: Maybe GetOrCreate special case. Retrieve by date, or retrieve record prior to date, or brand new snapshot.
                // This is currently not pulling the previous record to keep the snapshot rolling
                var poolSnapshotsQuery = new RetrieveActiveLiquidityPoolSnapshotsByPoolIdQuery(pool.Id, block.MedianTime);
                var poolSnapshots = await _mediator.Send(poolSnapshotsQuery, CancellationToken.None);

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
                                poolSnapshot.ProcessStakingLog((StartStakingLog)poolLog, odxSnapshot);
                                break;
                            case TransactionLogType.StopStakingLog:
                                poolSnapshot.ProcessStakingLog((StopStakingLog)poolLog, odxSnapshot);
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
            var tokenSnapshotQuery = new RetrieveTokenSnapshotsByTokenIdAndMarketIdAndTimeQuery(pool.Token.Id, pool.Market.Id, snapshotStart);
            var poolTokenSnapshots = await _mediator.Send(tokenSnapshotQuery, CancellationToken.None);
            
            var poolTokenSnapshot = poolTokenSnapshots.SingleOrDefault(s => s.SnapshotType == snapshotType) ??
                                    new TokenSnapshot(pool.Token.Id, pool.Market.Id, 0m, snapshotType, snapshotStart, snapshotEnd);
                    
            poolTokenSnapshot.ProcessReservesLog(log, crsSnapshot, pool.Token.Decimals);

            await _mediator.Send(new MakeTokenSnapshotCommand(poolTokenSnapshot), CancellationToken.None);
        }
    }
}