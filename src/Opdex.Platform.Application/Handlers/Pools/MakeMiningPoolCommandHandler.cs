using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Pools;
using Opdex.Platform.Application.Abstractions.Commands.Pools;
using Opdex.Platform.Domain.Models.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Pools;

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

            var miningPool = new MiningPool(request.LiquidityPoolId, request.MiningPool, pool.RewardRate, "0", pool.MiningPeriodEnd);
            
            return await _mediator.Send(new PersistMiningPoolCommand(miningPool), cancellationToken);
        }
    }
}