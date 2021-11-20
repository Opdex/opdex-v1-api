using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Markets;
using Opdex.Platform.Application.Abstractions.EntryCommands.Markets.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.Markets.Snapshots;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Markets.Snapshots
{
    public class ProcessMarketSnapshotsCommandHandler : IRequestHandler<ProcessMarketSnapshotsCommand, Unit>
    {
        private readonly IMediator _mediator;
        private const SnapshotType SnapshotType = Common.Enums.SnapshotType.Daily;

        public ProcessMarketSnapshotsCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<Unit> Handle(ProcessMarketSnapshotsCommand request, CancellationToken cancellationToken)
        {
            var marketPools = await _mediator.Send(new RetrieveLiquidityPoolsWithFilterQuery(new LiquidityPoolsCursor(request.Market.Address)));
            var marketSnapshot = await _mediator.Send(new RetrieveMarketSnapshotWithFilterQuery(request.Market.Id, request.BlockTime, SnapshotType));

            // Reset stale snapshot if its old
            if (marketSnapshot.EndDate < request.BlockTime)
            {
                marketSnapshot.ResetStaleSnapshot(request.BlockTime);
            }
            else
            {
                // Snapshots add (+=) LP snapshot values requiring reset each time its values are calculated
                marketSnapshot.ResetCurrentSnapshot();
            }

            // Todo: Task.WhenAll in chunks to get all liquidity pool's snapshots
            // During chunks add to ottal list of snapshots
            // Update market snapshot at once with agg totals, vs one at a time.

            // Each pool in the market
            foreach (var pool in marketPools)
            {
                // Get snapshot
                var poolSnapshot = await _mediator.Send(new RetrieveLiquidityPoolSnapshotWithFilterQuery(pool.Id, request.BlockTime, SnapshotType));

                // Skip if the returned snapshot is a new default
                if (poolSnapshot.Id == 0) continue;

                // Apply LP snapshot to market snapshot
                marketSnapshot.Update(poolSnapshot);
            }

            // Persist market snapshot
            await _mediator.Send(new MakeMarketSnapshotCommand(marketSnapshot));

            return Unit.Value;
        }
    }
}
