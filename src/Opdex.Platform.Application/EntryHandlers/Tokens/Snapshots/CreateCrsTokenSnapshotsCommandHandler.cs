using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Snapshots;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Feeds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Tokens.Snapshots;

public class CreateCrsTokenSnapshotsCommandHandler : IRequestHandler<CreateCrsTokenSnapshotsCommand, bool>
{
    private readonly IFiatPriceFeed _fiatPriceFeed;
    private readonly IMediator _mediator;
    private readonly ILogger<CreateCrsTokenSnapshotsCommandHandler> _logger;

    private const ulong CrsMarketId = 0;
    private readonly SnapshotType[] _snapshotTypes = { SnapshotType.Minute, SnapshotType.Hourly, SnapshotType.Daily };

    public CreateCrsTokenSnapshotsCommandHandler(IFiatPriceFeed fiatPriceFeed, IMediator mediator, ILogger<CreateCrsTokenSnapshotsCommandHandler> logger)
    {
        _fiatPriceFeed = fiatPriceFeed ?? throw new ArgumentNullException(nameof(fiatPriceFeed));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(CreateCrsTokenSnapshotsCommand request, CancellationToken cancellationToken)
    {
        var crs = await _mediator.Send(new RetrieveTokenByAddressQuery(Address.Cirrus, findOrThrow: false), CancellationToken.None);
        ulong crsId = crs?.Id ?? 0;

        // If CRS doesn't exist, create it
        if (crsId == 0)
        {
            crs = new Token(Address.Cirrus, TokenConstants.Cirrus.Name, TokenConstants.Cirrus.Symbol, TokenConstants.Cirrus.Decimals,
                            TokenConstants.Cirrus.Sats, TokenConstants.Cirrus.TotalSupply, request.BlockHeight);

            crsId = await _mediator.Send(new MakeTokenCommand(crs, request.BlockHeight), CancellationToken.None);

            if (crsId == 0) throw new Exception("Unable to create CRS token during create snapshot.");
        }

        var latestSnapshot = await _mediator.Send(new RetrieveTokenSnapshotWithFilterQuery(crsId, CrsMarketId, request.BlockTime, SnapshotType.Minute));

        // We expected to find a latest snapshot for the minute, if the end date is greater than our block time and it has an Id,
        // then we've already snapshot this minute, return successfully
        if (latestSnapshot.EndDate > request.BlockTime && latestSnapshot.Id > 0) return true;

        var price = await _fiatPriceFeed.GetCrsUsdPrice(request.BlockTime, CancellationToken.None);
        if (price <= 0m) return false;

        var results = await Task.WhenAll(_snapshotTypes.Select(async snapshotType =>
        {
            var snapshotOfType = await _mediator.Send(new RetrieveTokenSnapshotWithFilterQuery(crsId, CrsMarketId, request.BlockTime, snapshotType));

            if (snapshotOfType.EndDate < request.BlockTime) snapshotOfType.ResetStaleSnapshot(price, request.BlockTime);
            else snapshotOfType.UpdatePrice(price);

            var persisted = await _mediator.Send(new MakeTokenSnapshotCommand(snapshotOfType, request.BlockHeight));
            if (persisted) return true;

            using (_logger.BeginScope(new Dictionary<string, object>
                   {
                       { "SnapshotType", snapshotType },
                       { "BlockTime", request.BlockTime }
                   }))
            {
                _logger.LogError($"Unable to persist CRS token snapshot");
            }
            return false;
        }));

        return results.All(result => result);
    }
}
