using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens.Snapshots;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Snapshots;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Application.EntryHandlers.Tokens
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

            var snapshotsQuery = new RetrieveTokenSnapshotsByTokenIdAndMarketIdAndTimeQuery(crs.Id, CrsMarketId, request.BlockTime);
            var snapshots = await _mediator.Send(snapshotsQuery, CancellationToken.None);

            // If we've already got a minute snapshot, skip
            if (snapshots.Any(s => s.SnapshotType == SnapshotType.Minute))
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
                await ProcessTokenSnapshot(snapshotType, request.BlockTime, snapshots, crs.Id, price);
            }

            return Unit.Value;
        }

        private async Task ProcessTokenSnapshot(SnapshotType snapshotType, DateTime blockTime, IEnumerable<TokenSnapshot> snapshots, long tokenId, decimal price)
        {
            var snapshot = snapshots.SingleOrDefault(s => s.SnapshotType == snapshotType) ??
                           new TokenSnapshot(tokenId, CrsMarketId, snapshotType, blockTime);

            snapshot.UpdatePrice(price);

            var persisted = await _mediator.Send(new MakeTokenSnapshotCommand(snapshot), CancellationToken.None);
            if (!persisted)
            {
                throw new Exception("Unable to persist token snapshot.");
            }
        }
    }
}