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

namespace Opdex.Platform.Application.EntryHandlers.Pools.Snapshots
{
    public class GetLiquidityPoolSnapshotsWithFilterQueryHandler : IRequestHandler<GetLiquidityPoolSnapshotsWithFilterQuery, IEnumerable<LiquidityPoolSnapshotDto>>
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
            var poolQuery = new RetrieveLiquidityPoolByAddressQuery(request.LiquidityPoolAddress, findOrThrow: true);
            var pool = await _mediator.Send(poolQuery, cancellationToken);

            var from = request.From ?? new DateTime(2021, 01, 01);
            var to = request.To ?? DateTime.UtcNow.Add(TimeSpan.FromDays(1));
            var difference = to.Subtract(from);
            var snapshotType = difference.Days > 365 ? SnapshotType.Daily : SnapshotType.Hourly;

            var query = new RetrieveLiquidityPoolSnapshotsWithFilterQuery(pool.Id, from, to, snapshotType);

            var poolSnapshots = await _mediator.Send(query, cancellationToken);

            var poolDtos = _mapper.Map<IEnumerable<LiquidityPoolSnapshotDto>>(poolSnapshots.OrderBy(p => p.StartDate));

            return poolDtos;
        }
    }
}