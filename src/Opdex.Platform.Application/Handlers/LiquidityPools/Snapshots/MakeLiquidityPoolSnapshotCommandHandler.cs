using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.LiquidityPools;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.LiquidityPools.Snapshots;

public class MakeLiquidityPoolSnapshotCommandHandler : IRequestHandler<MakeLiquidityPoolSnapshotCommand, bool>
{
    private readonly IMediator _mediator;

    public MakeLiquidityPoolSnapshotCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<bool> Handle(MakeLiquidityPoolSnapshotCommand request, CancellationToken cancellationToken)
    {
        // Update the liquidity pool summary using the daily snapshot
        if (request.Snapshot.SnapshotType == SnapshotType.Daily)
        {
            var summary = await _mediator.Send(new RetrieveLiquidityPoolSummaryByLiquidityPoolIdQuery(request.Snapshot.LiquidityPoolId,
                                                                                                      findOrThrow: false));

            summary ??= new LiquidityPoolSummary(request.Snapshot.LiquidityPoolId, request.BlockHeight);

            if (summary.ModifiedBlock <= request.BlockHeight)
            {
                summary.Update(request.Snapshot, request.BlockHeight);

                await _mediator.Send(new MakeLiquidityPoolSummaryCommand(summary));
            }
        }

        return await _mediator.Send(new PersistLiquidityPoolSnapshotCommand(request.Snapshot), cancellationToken);
    }
}