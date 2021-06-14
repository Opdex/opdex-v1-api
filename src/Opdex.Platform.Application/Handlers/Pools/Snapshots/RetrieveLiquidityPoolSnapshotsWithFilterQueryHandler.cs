using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Pools.Snapshots;
using Opdex.Platform.Domain.Models.Pools.Snapshot;
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

        public async Task<IEnumerable<LiquidityPoolSnapshot>> Handle(RetrieveLiquidityPoolSnapshotsWithFilterQuery request, CancellationToken cancellationToken)
        {
            var snapshotQuery = new SelectLiquidityPoolSnapshotsWithFilterQuery(request.LiquidityPoolId, request.StartDate, request.EndDate, request.SnapshotType);
            var snapshots = await _mediator.Send(snapshotQuery, cancellationToken);

            if (snapshots.Any())
            {
                return snapshots;
            }

            return snapshots;
            // Get previous
            // SelectLiquidityPoolSnapshotsWithFilter
        }
    }
}