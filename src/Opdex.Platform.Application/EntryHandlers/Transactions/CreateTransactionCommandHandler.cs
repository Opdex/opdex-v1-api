using System;
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
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Handlers.Pools;

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
            
            // Get latest CRS price
            
            // Get active market snapshots
            var marketSnapshots = await _mediator.Send(new RetrieveActiveMarketSnapshotsByMarketIdQuery(0, DateTime.UtcNow), CancellationToken.None);
            
            // Get active pool snapshots
            // get active token snapshots
            
            var marketCreatedLogs = cirrusTx.LogsOfType<MarketCreatedLog>(TransactionLogType.MarketCreatedLog);
            foreach (var log in marketCreatedLogs)
            {
                // Check our market deployer address, index only our created markets.
                // Create new market
            }
            
            var liquidityPoolCreatedLogs = cirrusTx.LogsOfType<LiquidityPoolCreatedLog>(TransactionLogType.LiquidityPoolCreatedLog);
            foreach (var log in liquidityPoolCreatedLogs)
            {
                var tokenId = await _mediator.Send(new MakeTokenCommand(log.Token), CancellationToken.None);
                var pairId = await _mediator.Send(new MakeLiquidityPoolCommand(log.Pool, tokenId), CancellationToken.None);
            }
            
            var miningPoolCreatedLogs = cirrusTx.LogsOfType<MiningPoolCreatedLog>(TransactionLogType.MiningPoolCreatedLog);
            foreach (var log in miningPoolCreatedLogs)
            {
                var pool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(log.StakingPool), CancellationToken.None);
                var miningPoolId = await _mediator.Send(new MakeMiningPoolCommand(log.MiningPool, pool.Id), CancellationToken.None);
            }

            var reservesLogs = cirrusTx.LogsOfType<ReservesLog>(TransactionLogType.ReservesLog);
            foreach (var log in reservesLogs)
            {
                var pool = await _mediator.Send(new GetLiquidityPoolByAddressQuery(log.Contract), CancellationToken.None);
                var tokenSnapshots = await _mediator.Send(new RetrieveActiveTokenSnapshotsByTokenIdQuery(pool.Token.Id), CancellationToken.None);
                // Update token snapshots of price between reserves
                
                // - update pool snapshot reserves
                // - update pool/market snapshot volume
                // - update pool/market snapshot liquidity
                var poolSnapshots = await _mediator.Send(new RetrieveActiveLiquidityPoolSnapshotsByPoolIdQuery(0, DateTime.UtcNow), CancellationToken.None);

            }
            
            if (cirrusTx.Logs.Any(e => new [] { TransactionLogType.StartStakingLog, TransactionLogType.StopStakingLog}.Contains(e.LogType)))
            {
                // - update snapshot/market staking weight
            }
            
            return true;
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