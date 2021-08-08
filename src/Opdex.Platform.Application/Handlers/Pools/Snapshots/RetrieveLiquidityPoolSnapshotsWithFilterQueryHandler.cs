using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Pools.Snapshots;
using Opdex.Platform.Domain.Models.Pools.Snapshots;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pools.Snapshots;

namespace Opdex.Platform.Application.Handlers.Pools.Snapshots
{
    public class RetrieveLiquidityPoolSnapshotsWithFilterQueryHandler
        : IRequestHandler<RetrieveLiquidityPoolSnapshotsWithFilterQuery, IEnumerable<LiquidityPoolSnapshot>>
    {
        private readonly IMediator _mediator;

        public RetrieveLiquidityPoolSnapshotsWithFilterQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<IEnumerable<LiquidityPoolSnapshot>> Handle(RetrieveLiquidityPoolSnapshotsWithFilterQuery request,
                                                               CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectLiquidityPoolSnapshotsWithFilterQuery(request.LiquidityPoolId,
                                                                                  request.StartDate,
                                                                                  request.EndDate,
                                                                                  request.SnapshotType), cancellationToken);
        }
    }
}
