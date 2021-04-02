using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Core.Application.Abstractions.Models;
using Opdex.Core.Application.Abstractions.Queries.Blocks;
using Opdex.Core.Application.Abstractions.Queries.Transactions;
using Opdex.Core.Application.Assemblers;
using Opdex.Core.Domain.Models;
using Opdex.Core.Domain.Models.TransactionEvents;
using Opdex.Indexer.Application.Abstractions.Commands.Pools;
using Opdex.Indexer.Application.Abstractions.Commands.Tokens;
using Opdex.Indexer.Application.Abstractions.Commands.Transactions;
using Opdex.Indexer.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Indexer.Application.Abstractions.Queries.Transactions;

namespace Opdex.Indexer.Application.EntryHandlers.Transactions
{
    public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, bool>
    {
        private IMapper _mapper;
        private IMediator _mediator;
        private ILogger _logger;
        private IModelAssembler<Transaction, TransactionDto> _assembler;
        
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
            
            // Todo: Break this out of this handler
            // Create token and pair if necessary
            if (cirrusTx.Events.FirstOrDefault(e => e.EventType == nameof(PoolCreatedEvent)) is PoolCreatedEvent poolCreatedEvent)
            {
                var tokenId = await _mediator.Send(new MakeTokenCommand(poolCreatedEvent.Token));
                var pairId = await _mediator.Send(new MakePoolCommand(poolCreatedEvent.Pool, tokenId));
            }
            else
            {
                var latestBlock = await _mediator.Send(new RetrieveLatestBlockQuery());
                    
                if (latestBlock.Height == cirrusTx.BlockHeight)
                {
                    if (cirrusTx.Events.Any(e => e.EventType == nameof(SyncEvent)))
                    {
                        // - update pool reserves if sync event occurred
                    }
                    
                    if (cirrusTx.Events.Any(e => new [] { nameof(MintEvent), nameof(BurnEvent)}.Contains(e.EventType)))
                    {
                        // - update pool total supply if mint or burn event occurred
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