using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Application.Abstractions.Commands.Pools;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;

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
            var transactionDto = await TryGetExistingTransaction(request.TxHash, cancellationToken);

            if (transactionDto != null) return true;

            var cirrusTx = await _mediator.Send(new RetrieveCirrusTransactionByHashQuery(request.TxHash), cancellationToken);

            if (cirrusTx == null) return false;
            
            var result = await _mediator.Send(new MakeTransactionCommand(cirrusTx));

            if (!result) return false;

            if (cirrusTx.Logs.Any(e => e.LogType == nameof(LiquidityPoolCreatedLog)))
            {
                foreach (var log in cirrusTx.Logs.Where(e => e.LogType == nameof(LiquidityPoolCreatedLog)))
                {
                    var liquidityPoolLog = log as LiquidityPoolCreatedLog;
                    var tokenId = await _mediator.Send(new MakeTokenCommand(liquidityPoolLog.Token));
                    var pairId = await _mediator.Send(new MakeLiquidityPoolCommand(liquidityPoolLog.Pool, tokenId));
                }
            }
            else if (cirrusTx.Logs.Any(e => e.LogType == nameof(MiningPoolCreatedLog)))
            {
                foreach (var log in cirrusTx.Logs.Where(e => e.LogType == nameof(MiningPoolCreatedLog)))
                {
                    var miningPoolLog = log as MiningPoolCreatedLog;
                    var pool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(miningPoolLog.StakingPool));
                    var miningPoolId = await _mediator.Send(new MakeMiningPoolCommand(miningPoolLog.MiningPool, pool.Id));
                }
            }
            else
            {
                var latestBlock = await _mediator.Send(new RetrieveLatestBlockQuery());
                    
                if (latestBlock.Height == cirrusTx.BlockHeight)
                {
                    if (cirrusTx.Logs.Any(e => e.LogType == nameof(ReservesLog)))
                    {
                        // - update pool reserves if sync log occurred
                    }
                    
                    if (cirrusTx.Logs.Any(e => new [] { nameof(MintLog), nameof(BurnLog)}.Contains(e.LogType)))
                    {
                        // - update pool total supply if mint or burn log occurred
                    }
                        
                    // Todo: Update any other relevant data sets
                }
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