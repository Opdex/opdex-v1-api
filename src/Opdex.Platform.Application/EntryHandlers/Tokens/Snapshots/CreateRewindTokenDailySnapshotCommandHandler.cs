using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Snapshots;
using Opdex.Platform.Common.Enums;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Tokens.Snapshots
{
    public class CreateRewindTokenDailySnapshotCommandHandler : IRequestHandler<CreateRewindTokenDailySnapshotCommand, bool>
    {
        private readonly IMediator _mediator;

        public CreateRewindTokenDailySnapshotCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(CreateRewindTokenDailySnapshotCommand request, CancellationToken cancellationToken)
        {
            // Get the daily token snapshot to be rewound
            var srcTokenDailySnapshot = await _mediator.Send(new RetrieveTokenSnapshotWithFilterQuery(request.TokenId, request.MarketId,
                                                                                                      request.StartDate, SnapshotType.Daily));

            // Get existing hourly snapshots for the token
            var srcTokenHourlySnapshots = await _mediator.Send(new RetrieveTokenSnapshotsWithFilterQuery(request.TokenId, request.MarketId,
                                                                                                         request.StartDate, request.EndDate,
                                                                                                         SnapshotType.Hourly));

            // Rewind daily snapshot using hourly snapshots
            srcTokenDailySnapshot.RewindDailySnapshot(srcTokenHourlySnapshots.ToList());

            // Persist and return success
            return await _mediator.Send(new MakeTokenSnapshotCommand(srcTokenDailySnapshot));
        }
    }
}
