using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Pools.Snapshots;
using Opdex.Platform.Application.Abstractions.Models.PoolDtos;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Application.Abstractions.Queries.Pools.Snapshots;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.EntryHandlers.Pools.Snapshots
{
    public class GetLiquidityPoolSnapshotsWithFilterQueryHandler
        : IRequestHandler<GetLiquidityPoolSnapshotsWithFilterQuery, IEnumerable<LiquidityPoolSnapshotDto>>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public GetLiquidityPoolSnapshotsWithFilterQueryHandler(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<LiquidityPoolSnapshotDto>> Handle(GetLiquidityPoolSnapshotsWithFilterQuery request, CancellationToken cancellationToken)
        {
            var pool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(request.LiquidityPoolAddress), cancellationToken);

            var from = request.From ?? new DateTime(2021, 06, 11);
            var to = request.To ?? DateTime.UtcNow;
            var difference = to.Subtract(from);
            var snapshotType = difference.Days > 100 ? SnapshotType.Daily : SnapshotType.Hourly;

            var poolSnapshots = await _mediator.Send(new RetrieveLiquidityPoolSnapshotsWithFilterQuery(pool.Id,
                                                                                                       from,
                                                                                                       to.ToEndOf(snapshotType),
                                                                                                       snapshotType), cancellationToken);

            return _mapper.Map<IEnumerable<LiquidityPoolSnapshotDto>>(poolSnapshots.OrderBy(p => p.StartDate));
        }
    }
}
