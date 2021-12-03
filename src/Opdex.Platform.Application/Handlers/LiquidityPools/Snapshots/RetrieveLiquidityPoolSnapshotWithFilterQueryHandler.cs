using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools.Snapshots;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools.Snapshots;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.LiquidityPools.Snapshots;

public class RetrieveLiquidityPoolSnapshotWithFilterQueryHandler
    : IRequestHandler<RetrieveLiquidityPoolSnapshotWithFilterQuery, LiquidityPoolSnapshot>
{
    private readonly IMediator _mediator;

    public RetrieveLiquidityPoolSnapshotWithFilterQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public Task<LiquidityPoolSnapshot> Handle(RetrieveLiquidityPoolSnapshotWithFilterQuery request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new SelectLiquidityPoolSnapshotWithFilterQuery(request.LiquidityPoolId,
                                                                             request.DateTime,
                                                                             request.SnapshotType), cancellationToken);
    }
}