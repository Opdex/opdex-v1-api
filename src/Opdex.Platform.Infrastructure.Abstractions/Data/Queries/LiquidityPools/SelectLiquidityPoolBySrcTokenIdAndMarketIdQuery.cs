using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.LiquidityPools;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools;

public class SelectLiquidityPoolBySrcTokenIdAndMarketIdQuery : FindQuery<LiquidityPool>
{
    public SelectLiquidityPoolBySrcTokenIdAndMarketIdQuery(ulong tokenId, ulong marketId, bool findOrThrow = true) : base(findOrThrow)
    {
        if (tokenId < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(tokenId));
        }

        if (marketId < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(marketId));
        }

        TokenId = tokenId;
        MarketId = marketId;
    }

    public ulong TokenId { get; }
    public ulong MarketId { get; }
}