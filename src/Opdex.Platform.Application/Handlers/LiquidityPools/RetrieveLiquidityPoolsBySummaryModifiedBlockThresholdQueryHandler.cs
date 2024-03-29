using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.LiquidityPools;

public class RetrieveLiquidityPoolsBySummaryModifiedBlockThresholdQueryHandler
    : IRequestHandler<RetrieveLiquidityPoolsBySummaryModifiedBlockThresholdQuery, IEnumerable<LiquidityPool>>
{
    private readonly IMediator _mediator;

    public RetrieveLiquidityPoolsBySummaryModifiedBlockThresholdQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public Task<IEnumerable<LiquidityPool>> Handle(RetrieveLiquidityPoolsBySummaryModifiedBlockThresholdQuery request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new SelectLiquidityPoolsBySummaryModifiedBlockThresholdQuery(request.BlockThreshold), cancellationToken);
    }
}
