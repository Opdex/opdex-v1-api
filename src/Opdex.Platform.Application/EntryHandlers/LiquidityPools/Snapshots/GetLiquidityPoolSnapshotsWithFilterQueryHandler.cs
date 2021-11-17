using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.LiquidityPools.Snapshots;
using Opdex.Platform.Application.Abstractions.Models.LiquidityPools.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools.Snapshots;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.LiquidityPools.Snapshots
{
    public class GetLiquidityPoolSnapshotsWithFilterQueryHandler : EntryFilterQueryHandler<GetLiquidityPoolSnapshotsWithFilterQuery, LiquidityPoolSnapshotsDto>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public GetLiquidityPoolSnapshotsWithFilterQueryHandler(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override async Task<LiquidityPoolSnapshotsDto> Handle(GetLiquidityPoolSnapshotsWithFilterQuery request, CancellationToken cancellationToken)
        {
            var pool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(request.LiquidityPool), cancellationToken);

            var snapshots = await _mediator.Send(new RetrieveLiquidityPoolSnapshotsWithFilterQuery(pool.Id, request.Cursor), cancellationToken);

            var snapshotsResults = snapshots.ToList();

            var cursorDto = BuildCursorDto(snapshotsResults, request.Cursor, pointerSelector: result => (result.StartDate, result.Id));

            var assembledResults = snapshotsResults.Select(snapshot => _mapper.Map<LiquidityPoolSnapshotDto>(snapshot)).ToList();

            return new LiquidityPoolSnapshotsDto { Snapshots = assembledResults, Cursor = cursorDto };
        }
    }
}
