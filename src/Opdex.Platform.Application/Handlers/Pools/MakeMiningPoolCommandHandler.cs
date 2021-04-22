using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Pools;
using Opdex.Platform.Application.Abstractions.Commands.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands;

namespace Opdex.Platform.Application.Handlers.Pools
{
    public class MakeMiningPoolCommandHandler : IRequestHandler<MakeMiningPoolCommand, long>
    {
        private readonly IMediator _mediator;

        public MakeMiningPoolCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task<long> Handle(MakeMiningPoolCommand request, CancellationToken cancellationToken)
        {
            var pool = await _mediator.Send(new CallCirrusGetOpdexMiningPoolByAddressQuery(request.MiningPool), cancellationToken);
            
            pool.SetLiquidityPoolId(request.LiquidityPoolId);
            
            return await _mediator.Send(new PersistMiningPoolCommand(pool), cancellationToken);
        }
    }
}