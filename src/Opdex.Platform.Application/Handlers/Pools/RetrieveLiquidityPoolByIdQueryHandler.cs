using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Domain.Models.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pools;

namespace Opdex.Platform.Application.Handlers.Pools
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