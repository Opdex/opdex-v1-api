using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools.Summaries;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.LiquidityPools
{
    public class RetrieveLiquidityPoolSummaryByLiquidityPoolIdQueryHandler
        : IRequestHandler<RetrieveLiquidityPoolSummaryByLiquidityPoolIdQuery, LiquidityPoolSummary>
    {
        private readonly IMediator _mediator;

        public RetrieveLiquidityPoolSummaryByLiquidityPoolIdQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<LiquidityPoolSummary> Handle(RetrieveLiquidityPoolSummaryByLiquidityPoolIdQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectLiquidityPoolSummaryByLiquidityPoolIdQuery(request.LiquidityPoolId, request.FindOrThrow), cancellationToken);
        }
    }
}
