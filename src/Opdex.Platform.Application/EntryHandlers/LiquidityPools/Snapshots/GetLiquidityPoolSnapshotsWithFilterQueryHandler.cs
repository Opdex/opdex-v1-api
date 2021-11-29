using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.LiquidityPools.Snapshots;
using Opdex.Platform.Application.Abstractions.Models.LiquidityPools.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools.Snapshots;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.LiquidityPools.Snapshots
{
    public class GetLiquidityPoolSnapshotsWithFilterQueryHandler : EntryFilterQueryHandler<GetLiquidityPoolSnapshotsWithFilterQuery, LiquidityPoolSnapshotsDto>
    {
        private readonly IMediator _mediator;
        private readonly IModelAssembler<IList<LiquidityPoolSnapshot>, IEnumerable<LiquidityPoolSnapshotDto>> _assembler;

        public GetLiquidityPoolSnapshotsWithFilterQueryHandler(IMediator mediator,
                                                               IModelAssembler<IList<LiquidityPoolSnapshot>, IEnumerable<LiquidityPoolSnapshotDto>> assembler)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _assembler = assembler ?? throw new ArgumentNullException(nameof(assembler));
        }

        public override async Task<LiquidityPoolSnapshotsDto> Handle(GetLiquidityPoolSnapshotsWithFilterQuery request, CancellationToken cancellationToken)
        {
            var pool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(request.LiquidityPool), cancellationToken);

            var snapshots = await _mediator.Send(new RetrieveLiquidityPoolSnapshotsWithFilterQuery(pool.Id, request.Cursor), cancellationToken);

            var snapshotsResults = snapshots.ToList();

            var cursorDto = BuildCursorDto(snapshotsResults, request.Cursor, pointerSelector: result => (result.StartDate, result.Id));

            var assembledResults = await _assembler.Assemble(snapshotsResults);

            return new LiquidityPoolSnapshotsDto { Snapshots = assembledResults, Cursor = cursorDto };
        }
    }
}
