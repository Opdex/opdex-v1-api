using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.LiquidityPools;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.LiquidityPools
{
    public class RetrieveLiquidityPoolBySrcTokenIdAndMarketIdQuery : FindQuery<LiquidityPool>
    {
        public RetrieveLiquidityPoolBySrcTokenIdAndMarketIdQuery(ulong srcTokenId, ulong marketId, bool findOrThrow = true) : base(findOrThrow)
        {
            if (srcTokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(srcTokenId), $"{nameof(srcTokenId)} must be greater than 0.");
            }

            if (marketId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(marketId), $"{nameof(marketId)} must be greater than 0.");
            }

            SrcTokenId = srcTokenId;
            MarketId = marketId;
        }

        public ulong SrcTokenId { get; }
        public ulong MarketId { get; }
    }
}
