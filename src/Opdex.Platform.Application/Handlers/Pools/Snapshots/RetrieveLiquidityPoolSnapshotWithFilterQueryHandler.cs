using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Pools.Snapshots;
using Opdex.Platform.Domain.Models.Pools.Snapshots;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pools.Snapshots;

namespace Opdex.Platform.Application.Handlers.Pools.Snapshots
{
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
            var query = new SelectLiquidityPoolSnapshotWithFilterQuery(request.LiquidityPoolId, request.DateTime, request.SnapshotType);
            return _mediator.Send(query, cancellationToken);
        }
    }
}