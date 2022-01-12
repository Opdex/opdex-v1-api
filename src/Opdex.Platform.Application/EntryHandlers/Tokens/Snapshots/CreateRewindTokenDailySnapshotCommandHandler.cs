using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Snapshots;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Tokens.Snapshots;

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

        // Get existing hourly snapshots for the token in ASC order
        var cursor = new SnapshotCursor(Interval.OneHour, request.StartDate, request.EndDate, SortDirectionType.ASC, 24, PagingDirection.Forward, default);
        var srcTokenHourlySnapshots = await _mediator.Send(new RetrieveTokenSnapshotsWithFilterQuery(request.TokenId, request.MarketId, cursor));

        // If the rewind block is within the first hour of the day, that hourly snapshot will be deleted and none will exist.
        // Need to always reuse the latest state of the most recent found liquidity pool snapshot. Sometimes we might need to
        // reset stale snapshot prior to the rewind if no hourly are found.
        if (srcTokenDailySnapshot.IsStale(request.EndDate))
        {
            var token = await _mediator.Send(new RetrieveTokenByIdQuery(request.TokenId));

            var tokenAttributes = await _mediator.Send(new RetrieveTokenAttributesByTokenIdQuery(token.Id), CancellationToken.None);
            var isLpt = tokenAttributes.Select(attr => attr.AttributeType).Contains(TokenAttributeType.Provisional);

            // Liquidity pool tokens share the same address as liquidity pools, SRC tokens do not.
            var liquidityPool = isLpt
                ? await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(token.Address))
                : await _mediator.Send(new RetrieveLiquidityPoolBySrcTokenIdAndMarketIdQuery(request.TokenId, request.MarketId));

            var liquidityPoolSnapshot = await _mediator.Send(new RetrieveLiquidityPoolSnapshotWithFilterQuery(liquidityPool.Id, request.StartDate, SnapshotType.Daily));

            if (isLpt)
            {
                // Calc OLPT price based on total reserves USD / OLPT total supply
                var tokenPrice = MathExtensions.FiatPerToken(token.TotalSupply, liquidityPoolSnapshot.Reserves.Usd.Close, token.Sats);
                srcTokenDailySnapshot.ResetStaleSnapshot(tokenPrice, request.StartDate);
            }
            else
            {
                // Calc SRC price based on CrsPerSrc ratio and CRS USD price
                srcTokenDailySnapshot.ResetStaleSnapshot(liquidityPoolSnapshot.Cost.CrsPerSrc.Open, request.CrsUsdStartOfDay, request.StartDate);
            }
        }

        // Rewind daily snapshot using hourly snapshots
        srcTokenDailySnapshot.RewindDailySnapshot(srcTokenHourlySnapshots.ToList());

        // Persist and return success
        return await _mediator.Send(new MakeTokenSnapshotCommand(srcTokenDailySnapshot, request.BlockHeight));
    }
}
