using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Summaries;

namespace Opdex.Platform.Application.Handlers.Tokens.Snapshots;

public class MakeTokenSnapshotCommandHandler : IRequestHandler<MakeTokenSnapshotCommand, bool>
{
    private readonly IMediator _mediator;

    public MakeTokenSnapshotCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<bool> Handle(MakeTokenSnapshotCommand request, CancellationToken cancellationToken)
    {
        if (request.Snapshot.SnapshotType == SnapshotType.Daily)
        {
            var summary = await _mediator.Send(new SelectTokenSummaryByMarketAndTokenIdQuery(request.Snapshot.MarketId,
                                                                                             request.Snapshot.TokenId,
                                                                                             findOrThrow: false));

            summary ??= new TokenSummary(request.Snapshot.MarketId, request.Snapshot.TokenId, request.BlockHeight);

            if (summary.ModifiedBlock <= request.BlockHeight)
            {
                summary.Update(request.Snapshot, request.BlockHeight);
                await _mediator.Send(new PersistTokenSummaryCommand(summary));
            }
        }

        return await _mediator.Send(new PersistTokenSnapshotCommand(request.Snapshot), CancellationToken.None);
    }
}