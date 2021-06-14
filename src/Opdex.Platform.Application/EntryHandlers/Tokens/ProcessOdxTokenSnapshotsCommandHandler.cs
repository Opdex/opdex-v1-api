using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.Vault;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Application.EntryHandlers.Tokens
{
    public class ProcessOdxTokenSnapshotsCommandHandler : IRequestHandler<ProcessOdxTokenSnapshotsCommand, Unit>
    {
        private readonly IMediator _mediator;
        private readonly SnapshotType[] _snapshotTypes = { SnapshotType.Minute, SnapshotType.Hourly, SnapshotType.Daily };

        public ProcessOdxTokenSnapshotsCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<Unit> Handle(ProcessOdxTokenSnapshotsCommand request, CancellationToken cancellationToken)
        {
            var vaultQuery = new RetrieveVaultQuery(findOrThrow: true);
            var vault = await _mediator.Send(vaultQuery, CancellationToken.None);

            var odxQuery = new RetrieveTokenByIdQuery(vault.TokenId, findOrThrow: true);
            var odx = await _mediator.Send(odxQuery, CancellationToken.None);

            // Todo: Need to get the staking market
            // really, we need "GetStakingMarket" "GetStakingToken"
            const long marketId = 1;

            var snapshotsQuery = new RetrieveTokenSnapshotsByTokenIdAndMarketIdAndTimeQuery(odx.Id, marketId, request.BlockTime);
            var snapshots = await _mediator.Send(snapshotsQuery, CancellationToken.None);

            // If we've already got a minute snapshot, skip
            if (snapshots.Any(s => s.SnapshotType == SnapshotType.Minute))
            {
                return Unit.Value;
            }

            foreach (var snapshotType in _snapshotTypes)
            {
                // Todo: Need to calculate price
                var price = 0m;
                await ProcessTokenSnapshot(snapshotType, request.BlockTime, snapshots, odx.Id, price, marketId);
            }

            return Unit.Value;
        }

        private async Task ProcessTokenSnapshot(SnapshotType snapshotType, DateTime blockTime, IEnumerable<TokenSnapshot> snapshots, long tokenId, decimal price, long marketId)
        {
            var snapshot = snapshots.SingleOrDefault(s => s.SnapshotType == snapshotType) ??
                           new TokenSnapshot(tokenId, marketId, snapshotType, blockTime);

            snapshot.UpdatePrice(price);

            var persisted = await _mediator.Send(new MakeTokenSnapshotCommand(snapshot), CancellationToken.None);
            if (!persisted)
            {
                throw new Exception("Unable to persist token snapshot.");
            }
        }
    }
}