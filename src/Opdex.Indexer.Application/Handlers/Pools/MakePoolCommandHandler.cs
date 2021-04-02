using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Pools;
using Opdex.Indexer.Application.Abstractions.Commands.Pools;
using Opdex.Indexer.Infrastructure.Abstractions.Data.Commands;

namespace Opdex.Indexer.Application.Handlers.Pools
{
    public class MakePoolCommandHandler : IRequestHandler<MakePoolCommand, long>
    {
        private readonly IMediator _mediator;

        public MakePoolCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task<long> Handle(MakePoolCommand request, CancellationToken cancellationToken)
        {
            var pool = await _mediator.Send(new CallCirrusGetOpdexPoolByAddressQuery(request.Address), cancellationToken);
            
            pool.SetTokenId(request.TokenId);
            
            return await _mediator.Send(new PersistPoolCommand(pool), cancellationToken);
        }
    }
}