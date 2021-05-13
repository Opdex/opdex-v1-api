using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Domain.Models.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pools;

namespace Opdex.Platform.Application.Handlers.Pools
{
    public class RetrieveMiningPoolByLiquidityPoolIdQueryHandler : IRequestHandler<RetrieveMiningPoolByLiquidityPoolIdQuery, MiningPool>
    {
        private readonly IMediator _mediator;

        public RetrieveMiningPoolByLiquidityPoolIdQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public Task<MiningPool> Handle(RetrieveMiningPoolByLiquidityPoolIdQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectMiningPoolByLiquidityPoolIdQuery(request.LiquidityPoolId), cancellationToken);
        }
    }
}