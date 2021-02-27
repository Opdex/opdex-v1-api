using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;
using Opdex.Indexer.Application.Abstractions.Commands.Tokens;
using Opdex.Indexer.Infrastructure.Abstractions.Data.Commands;

namespace Opdex.Indexer.Application.Handlers.Tokens
{
    public class MakeTokenCommandHandler : IRequestHandler<MakeTokenCommand, long>
    {
        private readonly IMediator _mediator;

        public MakeTokenCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task<long> Handle(MakeTokenCommand request, CancellationToken cancellationToken)
        {
            var token = await _mediator.Send(new CallCirrusGetSrcTokenDetailsByAddressQuery(request.Address), cancellationToken);
            
            return await _mediator.Send(new PersistTokenCommand(token), cancellationToken);
        }
    }
}