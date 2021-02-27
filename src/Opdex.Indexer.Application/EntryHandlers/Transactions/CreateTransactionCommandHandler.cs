using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
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
        
        public CreateTransactionCommandHandler(IMapper mapper, IMediator mediator,
            ILogger<CreateTransactionCommandHandler> logger)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<bool> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
        {
            var tx = await _mediator.Send(new RetrieveCirrusTransactionByHashQuery(request.TxHash), cancellationToken);
            
            // 
            // Todo: Return TransactionDto
            var result = await _mediator.Send(new MakeTransactionCommand(tx));

            // Check TransactionDto for PairCreated event
            // - If exists - add token - add pair
            
            // Get Latest Block, Compare to Tx Block
            // If latestBlock == txBlock
            // - update pair reserves if new or sync event occurred
            // - update pair total supply if mint or burn event occurred
            
            return result;
        }
    }
}