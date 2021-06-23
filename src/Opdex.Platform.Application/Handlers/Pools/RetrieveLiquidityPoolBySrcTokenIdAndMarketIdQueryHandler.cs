using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Domain.Models.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pools;

namespace Opdex.Platform.Application.Handlers.Pools
{
    public class RetrieveLiquidityPoolBySrcTokenIdAndMarketIdQueryHandler : IRequestHandler<RetrieveLiquidityPoolBySrcTokenIdAndMarketIdQuery, LiquidityPool>
    {
        private readonly IMediator _mediator;

        public RetrieveLiquidityPoolBySrcTokenIdAndMarketIdQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<LiquidityPool> Handle(RetrieveLiquidityPoolBySrcTokenIdAndMarketIdQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectLiquidityPoolBySrcTokenIdAndMarketIdQuery(request.SrcTokenId, request.MarketId, request.FindOrThrow), cancellationToken);
        }
    }
}