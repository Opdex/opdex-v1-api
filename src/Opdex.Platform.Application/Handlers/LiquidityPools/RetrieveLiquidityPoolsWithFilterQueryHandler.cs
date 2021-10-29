using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.LiquidityPools
{
    public class RetrieveLiquidityPoolsWithFilterQueryHandler : IRequestHandler<RetrieveLiquidityPoolsWithFilterQuery, IEnumerable<LiquidityPool>>
    {
        private readonly IMediator _mediator;

        public RetrieveLiquidityPoolsWithFilterQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<IEnumerable<LiquidityPool>> Handle(RetrieveLiquidityPoolsWithFilterQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectLiquidityPoolsWithFilterQuery(request.Cursor), cancellationToken);
        }
    }
}
