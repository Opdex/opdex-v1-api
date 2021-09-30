using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.LiquidityPools;
using Opdex.Platform.Application.Abstractions.EntryCommands.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools.Snapshots;
using Opdex.Platform.Common.Enums;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.LiquidityPools.Snapshots
{
    public class CreateRewindLiquidityPoolDailySnapshotCommandHandler : IRequestHandler<CreateRewindLiquidityPoolDailySnapshotCommand, bool>
    {
        private readonly IMediator _mediator;

        public CreateRewindLiquidityPoolDailySnapshotCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(CreateRewindLiquidityPoolDailySnapshotCommand request, CancellationToken cancellationToken)
        {
            // Get the current daily liquidity pool snapshot to be rewound
            var poolDailySnapshot = await _mediator.Send(new RetrieveLiquidityPoolSnapshotWithFilterQuery(request.LiquidityPoolId,
                                                                                                          request.StartDate,
                                                                                                          SnapshotType.Daily));

            // Get hourly liquidity pool snapshots to rebuild the daily
            var poolHourlySnapshots = await _mediator.Send(new RetrieveLiquidityPoolSnapshotsWithFilterQuery(request.LiquidityPoolId,
                                                                                                             request.StartDate,
                                                                                                             request.EndDate,
                                                                                                             SnapshotType.Hourly));

            // Rewind the daily snapshot using the hourly snapshots
            poolDailySnapshot.RewindDailySnapshot(poolHourlySnapshots.ToList());

            // Persist and return success
            return await _mediator.Send(new MakeLiquidityPoolSnapshotCommand(poolDailySnapshot));
        }
    }
}
