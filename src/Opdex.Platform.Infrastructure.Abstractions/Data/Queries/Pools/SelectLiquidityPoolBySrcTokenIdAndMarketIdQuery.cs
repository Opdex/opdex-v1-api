using System;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Pools;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pools
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