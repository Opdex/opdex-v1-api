using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens.Snapshots;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Snapshots;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Tokens.Snapshots
{
    public class CreateCrsTokenSnapshotsCommandHandler : IRequestHandler<CreateCrsTokenSnapshotsCommand, Unit>
    {
        private readonly IMediator _mediator;

        private const long CrsMarketId = 0;
        private readonly SnapshotType[] _snapshotTypes = { SnapshotType.Minute, SnapshotType.Hourly, SnapshotType.Daily };

        public CreateCrsTokenSnapshotsCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<Unit> Handle(CreateCrsTokenSnapshotsCommand request, CancellationToken cancellationToken)
        {
            var crs = await _mediator.Send(new GetTokenByAddressQuery(TokenConstants.Cirrus.Address), CancellationToken.None);

            var snapshot = await _mediator.Send(new RetrieveTokenSnapshotWithFilterQuery(crs.Id,
                                                                                         CrsMarketId,
                                                                                         request.BlockTime,
                                                                                         SnapshotType.Minute), CancellationToken.None);

            // If we've already got a minute snapshot, skip
            if (snapshot.EndDate > request.BlockTime)
            {
                return Unit.Value;
            }

            var isFiveMinutesOrOlder = DateTime.UtcNow.Subtract(request.BlockTime) > TimeSpan.FromMinutes(5);

            if (isFiveMinutesOrOlder)
            {
                // Should be getting historical if its an old transaction when we start paying for cmc
            }

            // Todo: Adjust this when we start getting historical prices
            var price = await _mediator.Send(new RetrieveCmcStraxPriceQuery(), CancellationToken.None);

            foreach (var snapshotType in _snapshotTypes)
            {
                var snapshotOfType = await _mediator.Send(new RetrieveTokenSnapshotWithFilterQuery(crs.Id,
                                                                                                   CrsMarketId,
                                                                                                   request.BlockTime,
                                                                                                   snapshotType), CancellationToken.None);

                // Reset stale snapshot or update a current one
                if (snapshotOfType.EndDate < request.BlockTime)
                {
                    snapshotOfType.ResetStaleSnapshot(price, request.BlockTime);
                }
                else
                {
                    snapshotOfType.UpdatePrice(price);
                }

                var persisted = await _mediator.Send(new MakeTokenSnapshotCommand(snapshot), CancellationToken.None);
                if (!persisted)
                {
                    throw new Exception("Unable to persist token snapshot.");
                }
            }

            return Unit.Value;
        }
    }
}
