using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pools;

namespace Opdex.Platform.Application.Handlers.Pools
{
    public class RetrieveActiveLiquidityPoolSnapshotsByPoolIdQueryHandler : IRequestHandler<RetrieveActiveLiquidityPoolSnapshotsByPoolIdQuery, IEnumerable<LiquidityPoolSnapshot>>
    {
        private readonly IMediator _mediator;
        
        private RetrieveActiveLiquidityPoolSnapshotsByPoolIdQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public Task<IEnumerable<LiquidityPoolSnapshot>> Handle(RetrieveActiveLiquidityPoolSnapshotsByPoolIdQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectActiveLiquidityPoolSnapshotsByPoolIdQuery(request.PoolId, request.Time), cancellationToken);
        }
    }
}