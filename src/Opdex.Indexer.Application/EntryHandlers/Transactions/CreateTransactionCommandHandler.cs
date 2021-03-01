using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Core.Application.Abstractions.Models;
using Opdex.Core.Application.Abstractions.Queries.Blocks;
using Opdex.Core.Application.Assemblers;
using Opdex.Core.Domain.Models;
using Opdex.Core.Domain.Models.TransactionEvents;
using Opdex.Indexer.Application.Abstractions.Commands.Pairs;
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
            TransactionDto transactionDto = null;
            
            try
            {
                var transaction = await _mediator.Send(new RetrieveTransactionByHashQuery(request.TxHash), cancellationToken);
                transactionDto = await _assembler.Assemble(transaction);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"{nameof(Transaction)} with hash {request.TxHash} is not found. Fetching from Cirrus to index.");
            }

            var cirrusTx = await _mediator.Send(new RetrieveCirrusTransactionByHashQuery(request.TxHash), cancellationToken);
            
            if (transactionDto == null)
            {
                var result = await _mediator.Send(new MakeTransactionCommand(cirrusTx));

                if (cirrusTx.Events.FirstOrDefault(e => e.EventType == nameof(PairCreatedEvent)) is PairCreatedEvent pairCreatedEvent)
                {
                    // Todo: Break this out of this handler, return TransactionDto from this handler, insert token/pair outside
                    var tokenId = await _mediator.Send(new MakeTokenCommand(pairCreatedEvent.Token));
                    await _mediator.Send(new MakePairCommand(pairCreatedEvent.Pair, tokenId));
                }
                else
                {
                    var latestBlock = await _mediator.Send(new RetrieveLatestBlockQuery());
                    
                    if (latestBlock.Height == cirrusTx.BlockHeight)
                    {
                        if (cirrusTx.Events.Any(e => e.EventType == nameof(SyncEvent)))
                        {
                            // - update pair reserves if sync event occurred
                        }
                    
                        if (cirrusTx.Events.Any(e => new [] { nameof(MintEvent), nameof(BurnEvent)}.Contains(e.EventType)))
                        {
                            // - update pair total supply if mint or burn event occurred
                        }
                        
                        // Todo: Update any other relevant data sets
                    }
                }
            }
            else
            {
                var missingEvents = cirrusTx.Events.Where(tx => transactionDto.Events.All(dto => dto.SortOrder != tx.SortOrder)).ToList();

                if (missingEvents.Any())
                {
                    // Insert missing events
                }
            }
            
            return true;
        }
    }
}