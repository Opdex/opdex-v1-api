using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Domain.Models.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pools;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Pools
{
    public class RetrieveLiquidityPoolByLpTokenIdQueryHandler : IRequestHandler<RetrieveLiquidityPoolByLpTokenIdQuery, LiquidityPool>
    {
        private readonly IMediator _mediator;

        public RetrieveLiquidityPoolByLpTokenIdQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<LiquidityPool> Handle(RetrieveLiquidityPoolByLpTokenIdQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectLiquidityPoolByLpTokenIdQuery(request.LpTokenId, request.FindOrThrow), cancellationToken);
        }
    }
}