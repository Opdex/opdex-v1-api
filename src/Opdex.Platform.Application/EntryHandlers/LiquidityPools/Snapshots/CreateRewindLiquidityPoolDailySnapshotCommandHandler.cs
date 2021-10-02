using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.LiquidityPools;
using Opdex.Platform.Application.Abstractions.EntryCommands.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
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

            // If the rewind block is within the first hour of the day, that hourly snapshot will be deleted and none will exist.
            // Need to always reuse the latest state of the most recent found daily liquidity pool snapshot. Sometimes we might need to
            // reset stale snapshot prior to the rewind if no hourly are found.
            if (poolDailySnapshot.IsStale(request.EndDate))
            {
                var srcToken = await _mediator.Send(new RetrieveTokenByIdQuery(request.SrcTokenId));

                // Liquidity pools reserves are always 50% USD CRS and 50% USD SRC - use CRS to get the total reserves
                var halfPoolReservesStartOfDay = poolDailySnapshot.Reserves.Crs.TotalFiat(request.CrsUsdStartOfDay, TokenConstants.Cirrus.Sats);
                var srcFiatPerTokenStartOfDay = poolDailySnapshot.Reserves.Src.FiatPerToken(halfPoolReservesStartOfDay, srcToken.Sats);

                // Reset the daily snapshot Id and date to be a new instance, carrying over necessary values from the previous snapshot.
                poolDailySnapshot.ResetStaleSnapshot(request.CrsUsdStartOfDay, srcFiatPerTokenStartOfDay,
                                                     request.StakingTokenUsdStartOfDay, srcToken.Sats, request.StartDate);
            }

            // Rewind the daily snapshot using the hourly snapshots
            poolDailySnapshot.RewindDailySnapshot(poolHourlySnapshots.ToList());

            // Persist and return success
            return await _mediator.Send(new MakeLiquidityPoolSnapshotCommand(poolDailySnapshot));
        }
    }
}
