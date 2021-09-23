using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Markets;
using Opdex.Platform.Application.Abstractions.EntryCommands.Markets.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Markets.Snapshots;
using Opdex.Platform.Common.Enums;
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
            var market = await _mediator.Send(new RetrieveMarketByIdQuery(request.MarketId));
            var marketPools = await _mediator.Send(new RetrieveLiquidityPoolsWithFilterQuery(market.Id));
            var marketSnapshot = await _mediator.Send(new RetrieveMarketSnapshotWithFilterQuery(request.MarketId,
                                                                                                request.BlockTime,
                                                                                                SnapshotType));

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

            // Each pool in the market
            foreach (var pool in marketPools)
            {
                // Get snapshot
                var poolSnapshot = await _mediator.Send(new RetrieveLiquidityPoolSnapshotWithFilterQuery(pool.Id,
                                                                                                         request.BlockTime,
                                                                                                         SnapshotType));

                // Apply LP snapshot to market snapshot
                marketSnapshot.Update(poolSnapshot);
            }

            // Persist market snapshot
            await _mediator.Send(new MakeMarketSnapshotCommand(marketSnapshot));

            return Unit.Value;
        }
    }
}
