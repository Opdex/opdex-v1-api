using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens.Snapshots;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Snapshots;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Tokens.Snapshots
{
    public class CreateCrsTokenSnapshotsCommandHandler : IRequestHandler<CreateCrsTokenSnapshotsCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CreateCrsTokenSnapshotsCommandHandler> _logger;

        private const ulong CrsMarketId = 0;
        private readonly SnapshotType[] _snapshotTypes = { SnapshotType.Minute, SnapshotType.Hourly, SnapshotType.Daily };

        public CreateCrsTokenSnapshotsCommandHandler(IMediator mediator, ILogger<CreateCrsTokenSnapshotsCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(CreateCrsTokenSnapshotsCommand request, CancellationToken cancellationToken)
        {
            var crs = await _mediator.Send(new GetTokenByAddressQuery(Address.Cirrus));

            var latestSnapshot = await _mediator.Send(new RetrieveTokenSnapshotWithFilterQuery(crs.Id, CrsMarketId, request.BlockTime, SnapshotType.Minute));

            if (latestSnapshot.EndDate > request.BlockTime) return true;

            var price = await _mediator.Send(new RetrieveCmcStraxPriceQuery(request.BlockTime));

            if (price <= 0m) return false;

            var results = await Task.WhenAll(_snapshotTypes.Select(async snapshotType =>
            {
                var snapshotOfType = await _mediator.Send(new RetrieveTokenSnapshotWithFilterQuery(crs.Id, CrsMarketId, request.BlockTime, snapshotType));

                if (snapshotOfType.EndDate < request.BlockTime) snapshotOfType.ResetStaleSnapshot(price, request.BlockTime);
                else snapshotOfType.UpdatePrice(price);

                var persisted = await _mediator.Send(new MakeTokenSnapshotCommand(snapshotOfType, request.BlockHeight));
                if (persisted) return true;

                _logger.LogError($"Unable to persist CRS token snapshot type {snapshotType} at block time: {request.BlockTime}");
                return false;
            }));

            return results.All(result => result);
        }
    }
}
