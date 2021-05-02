using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Application.Abstractions.Commands.Pools;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryQueries.Pools;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Deployers;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.EntryHandlers.Transactions
{
    public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, bool>
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ILogger _logger;
        private readonly IModelAssembler<Transaction, TransactionDto> _assembler;
        
        public CreateTransactionCommandHandler(IMapper mapper, IMediator mediator,
            ILogger<CreateTransactionCommandHandler> logger, IModelAssembler<Transaction, TransactionDto> assembler)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _assembler = assembler ?? throw new ArgumentNullException(nameof(assembler));
        }
        
        public async Task<bool> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
        {
            var transactionDto = await TryGetExistingTransaction(request.TxHash, CancellationToken.None);

            if (transactionDto != null) return true;

            var cirrusTx = await _mediator.Send(new RetrieveCirrusTransactionByHashQuery(request.TxHash), CancellationToken.None);

            if (cirrusTx == null) return false;
            
            var result = await _mediator.Send(new MakeTransactionCommand(cirrusTx), CancellationToken.None);

            if (!result) return false;

            // Process Market Created Logs
            var marketCreatedLogs = cirrusTx.LogsOfType<MarketCreatedLog>(TransactionLogType.MarketCreatedLog);
            foreach (var log in marketCreatedLogs)
            {
                // Check our market deployer address, index only our created markets.
                // var deployer = await _mediator.Send(new RetrieveDeployerByAddressQuery(log.Contract), CancellationToken.None);
                //
                // // Ignore if it's not one of Opdex deployers
                // if (deployer == null) continue;
                
                // Create new market
                // Todo: Need to know if its staking or not
                // Todo: Retrieve market first to see if it exists
                var marketId = await _mediator.Send(new MakeMarketCommand(log.Market, log.AuthPoolCreators, log.AuthProviders, log.AuthTraders, log.Fee, false), CancellationToken.None);
            }
            
            // Process Liquidity Pool Created Logs
            var liquidityPoolCreatedLogs = cirrusTx.LogsOfType<LiquidityPoolCreatedLog>(TransactionLogType.LiquidityPoolCreatedLog);
            foreach (var log in liquidityPoolCreatedLogs)
            {
                var marketI = await _mediator.Send(new RetrieveMarketByAddressQuery(log.Contract), CancellationToken.None);
                var tokenId = await _mediator.Send(new MakeTokenCommand(log.Token), CancellationToken.None);
                var pairId = await _mediator.Send(new MakeLiquidityPoolCommand(log.Pool, tokenId, marketI.Id), CancellationToken.None);
            }
            
            // Process Mining Pool Created Logs
            var miningPoolCreatedLogs = cirrusTx.LogsOfType<MiningPoolCreatedLog>(TransactionLogType.MiningPoolCreatedLog);
            foreach (var log in miningPoolCreatedLogs)
            {
                var pool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(log.StakingPool), CancellationToken.None);
                var miningPoolId = await _mediator.Send(new MakeMiningPoolCommand(log.MiningPool, pool.Id), CancellationToken.None);
            }

            // await ProcessSnapshots(cirrusTx);
            
            return true;
        }

        // Todo: This needs to be cleaned up and put in it's own command / handler
        // Still considering other approaches to this
        // This processes liquidity pool and token related snapshots for historical analytic purposes
        // Consider a snapshot for mining pools, is historical data ever needed or only current data?
        private async Task ProcessSnapshots(Transaction tx)
        {
            var crsToken = await _mediator.Send(new RetrieveTokenByAddressQuery("CRS"), CancellationToken.None);
            // Todo: Should not be latest, should be by snapshot time
            var crsSnapshot = await _mediator.Send(new RetrieveLatestTokenSnapshotByTokenIdQuery(crsToken.Id), CancellationToken.None);
            var block = await _mediator.Send(new RetrieveBlockByHeightQuery(tx.BlockHeight), CancellationToken.None);
            
            var poolSnapshotLogTypes = new List<TransactionLogType>
            {
                TransactionLogType.ReservesLog,
                TransactionLogType.SwapLog,
                TransactionLogType.StartStakingLog,
                TransactionLogType.StopStakingLog
            };
            
            var snapshotTypes = new[] {SnapshotType.Hourly, SnapshotType.Daily};

            var poolSnapshotGroups = tx.GroupedLogsOfTypes(poolSnapshotLogTypes);
            
            // Each pool in the dictionary
            foreach (var (poolContract, value) in poolSnapshotGroups)
            {
                var pool = await _mediator.Send(new GetLiquidityPoolByAddressQuery(poolContract), CancellationToken.None);
                var poolSnapshots = await _mediator.Send(new RetrieveActiveLiquidityPoolSnapshotsByPoolIdQuery(pool.Id, block.MedianTime), CancellationToken.None);

                // Each snapshot type we care about for pools
                foreach (var snapshotType in snapshotTypes)
                {
                    var snapshotStart = snapshotType == SnapshotType.Hourly ? block.MedianTime.StartOfHour() : block.MedianTime.StartOfDay();
                    var snapshotEnd = snapshotType == SnapshotType.Hourly ? block.MedianTime.EndOfHour() : block.MedianTime.EndOfDay();
                    var poolSnapshot = poolSnapshots.SingleOrDefault(s => s.SnapshotType == snapshotType) ??
                                       new LiquidityPoolSnapshot(pool.Id, snapshotType, snapshotStart, snapshotEnd);

                    // Each log to process
                    foreach (var poolLog in value)
                    {
                        switch (poolLog.LogType)
                        {
                            // Update pool liquidity(reserves) snapshot | CRS, SRC, USD
                            // Update src token price snapshots | USD
                            case TransactionLogType.ReservesLog:
                                var reservesLog = (ReservesLog)poolLog;
                                await ProcessTokenSnapshot(pool, snapshotType, reservesLog, snapshotStart, snapshotEnd);
                                poolSnapshot.ProcessReservesLog(reservesLog, crsSnapshot, crsToken);
                                break;
                            // - update pool volume snapshot | CRS, SRC, USD
                            // - update pool rewards snapshot | USD
                            case TransactionLogType.SwapLog:
                                poolSnapshot.ProcessSwapLog((SwapLog)poolLog, crsSnapshot, crsToken);
                                break;
                            // - update pool staking weight snapshot | SRC, USD
                            case TransactionLogType.StartStakingLog:
                                // Todo: Should be odx snapshot / token
                                poolSnapshot.ProcessStakingLog((StartStakingLog)poolLog, crsSnapshot, crsToken);
                                break;
                            // - update pool staking weight snapshot | SRC, USD
                            case TransactionLogType.StopStakingLog:
                                // Todo: Should be odx snapshot / token
                                poolSnapshot.ProcessStakingLog((StopStakingLog)poolLog, crsSnapshot, crsToken);
                                break;
                        }
                        
                        await _mediator.Send(new MakeLiquidityPoolSnapshotCommand(poolSnapshot), CancellationToken.None); 
                    }
                }
            }
        }
        
        private async Task ProcessTokenSnapshot(LiquidityPoolDto pool, SnapshotType snapshotType, ReservesLog log, DateTime snapshotStart, DateTime snapshotEnd)
        {
            var poolTokenSnapshots = await _mediator.Send(new RetrieveActiveTokenSnapshotsByTokenIdQuery(pool.Token.Id, snapshotStart), CancellationToken.None);

            var poolTokenSnapshot = poolTokenSnapshots.SingleOrDefault(s => s.SnapshotType == snapshotType) ??
                                    new TokenSnapshot(pool.Token.Id, 0m, snapshotType, snapshotStart, snapshotEnd);
                    
            poolTokenSnapshot.ProcessReservesLog(log);

            await _mediator.Send(new MakeTokenSnapshotCommand(poolTokenSnapshot), CancellationToken.None);
        }

        private async Task<TransactionDto> TryGetExistingTransaction(string txHash, CancellationToken cancellationToken)
        {
            TransactionDto transactionDto = null;
            
            try
            {
                var transaction = await _mediator.Send(new RetrieveTransactionByHashQuery(txHash), cancellationToken);
                transactionDto = await _assembler.Assemble(transaction);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, $"{nameof(Transaction)} with hash {txHash} is not found. Fetching from Cirrus to index.");
            }

            return transactionDto;
        }
    }
}