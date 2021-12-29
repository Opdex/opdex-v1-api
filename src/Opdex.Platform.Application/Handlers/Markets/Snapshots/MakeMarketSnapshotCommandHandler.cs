using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Markets;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Markets.Snapshots;

public class MakeMarketSnapshotCommandHandler : IRequestHandler<MakeMarketSnapshotCommand, bool>
{
    private readonly IMediator _mediator;

    public MakeMarketSnapshotCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<bool> Handle(MakeMarketSnapshotCommand request, CancellationToken cancellationToken)
    {
        // Update the liquidity pool summary using the daily snapshot
        if (request.Snapshot.SnapshotType == SnapshotType.Daily)
        {
            var summary = await _mediator.Send(new RetrieveMarketSummaryByMarketIdQuery(request.Snapshot.MarketId, findOrThrow: false));

            summary ??= new MarketSummary(request.Snapshot.MarketId, request.BlockHeight);

            if (summary.ModifiedBlock <= request.BlockHeight)
            {
                summary.Update(request.Snapshot, request.BlockHeight);

                await _mediator.Send(new MakeMarketSummaryCommand(summary));
            }
        }

        return await _mediator.Send(new PersistMarketSnapshotCommand(request.Snapshot), cancellationToken);
    }
}
