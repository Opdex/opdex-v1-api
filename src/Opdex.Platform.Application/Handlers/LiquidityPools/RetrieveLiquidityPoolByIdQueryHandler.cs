using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.LiquidityPools
{
    public class RetrieveLiquidityPoolByIdQueryHandler : IRequestHandler<RetrieveLiquidityPoolByIdQuery, LiquidityPool>
    {
        private readonly IMediator _mediator;

        public RetrieveLiquidityPoolByIdQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<LiquidityPool> Handle(RetrieveLiquidityPoolByIdQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectLiquidityPoolByIdQuery(request.LiquidityPoolId, request.FindOrThrow), cancellationToken);
        }
    }
}
