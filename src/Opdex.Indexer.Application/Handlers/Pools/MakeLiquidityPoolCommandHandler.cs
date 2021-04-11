using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Pools;
using Opdex.Indexer.Application.Abstractions.Commands.Pools;
using Opdex.Indexer.Infrastructure.Abstractions.Data.Commands;

namespace Opdex.Indexer.Application.Handlers.Pools
{
    public class MakeLiquidityPoolCommandHandler : IRequestHandler<MakeLiquidityPoolCommand, long>
    {
        private readonly IMediator _mediator;

        public MakeLiquidityPoolCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task<long> Handle(MakeLiquidityPoolCommand request, CancellationToken cancellationToken)
        {
            var pool = await _mediator.Send(new CallCirrusGetOpdexLiquidityPoolByAddressQuery(request.Address), cancellationToken);
            
            pool.SetTokenId(request.TokenId);
            
            return await _mediator.Send(new PersistLiquidityPoolCommand(pool), cancellationToken);
        }
    }
}