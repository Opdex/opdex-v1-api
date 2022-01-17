using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools.Snapshots;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools.Snapshots;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.LiquidityPools.Snapshots;

public class RetrieveStaleLiquidityPoolSnapshotsQueryHandler : IRequestHandler<RetrieveStaleLiquidityPoolSnapshotsQuery, IEnumerable<LiquidityPoolSnapshot>>
{
    private readonly IMediator _mediator;

    public RetrieveStaleLiquidityPoolSnapshotsQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public Task<IEnumerable<LiquidityPoolSnapshot>> Handle(RetrieveStaleLiquidityPoolSnapshotsQuery request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new SelectStaleLiquidityPoolSnapshotsQuery(), cancellationToken);
    }
}
