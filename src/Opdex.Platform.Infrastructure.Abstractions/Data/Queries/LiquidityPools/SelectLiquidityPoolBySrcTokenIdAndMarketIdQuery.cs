using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.LiquidityPools;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools
{
    public class SelectLiquidityPoolBySrcTokenIdAndMarketIdQuery : FindQuery<LiquidityPool>
    {
        public SelectLiquidityPoolBySrcTokenIdAndMarketIdQuery(long tokenId, long marketId, bool findOrThrow = true) : base(findOrThrow)
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

        public long TokenId { get; }
        public long MarketId { get; }
    }
}
