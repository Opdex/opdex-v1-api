using System;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Markets;

namespace Opdex.Platform.Application.Abstractions.Queries.Markets;

public class RetrieveActiveMarketRouterByMarketIdQuery : FindQuery<MarketRouter>
{
    public RetrieveActiveMarketRouterByMarketIdQuery(ulong marketId, bool findOrThrow = true) : base(findOrThrow)
    {
        if (marketId < 1)
        {
            throw new ArgumentNullException(nameof(marketId));
        }

        MarketId = marketId;
    }

    public ulong MarketId { get; }
}