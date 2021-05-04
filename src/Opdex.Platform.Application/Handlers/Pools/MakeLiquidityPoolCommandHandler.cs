using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Pools;
using Opdex.Platform.Application.Abstractions.Commands.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Pools;

namespace Opdex.Platform.Application.Handlers.Pools
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
            pool.SetMarketId(request.MarketId);
            
            return await _mediator.Send(new PersistLiquidityPoolCommand(pool), cancellationToken);
        }
    }
}