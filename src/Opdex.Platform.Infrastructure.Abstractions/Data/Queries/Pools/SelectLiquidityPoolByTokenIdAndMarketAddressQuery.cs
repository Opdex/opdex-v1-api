using System;
using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Pools;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pools
{
    public class SelectLiquidityPoolByTokenIdAndMarketAddressQuery : FindQuery<LiquidityPool>
    {
        public SelectLiquidityPoolByTokenIdAndMarketAddressQuery(long tokenId, string marketAddress, bool findOrThrow = true) : base(findOrThrow)
        {
            if (tokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenId));
            }
            
            if (!marketAddress.HasValue())
            {
                throw new ArgumentNullException(nameof(marketAddress));
            }

            TokenId = tokenId;
            MarketAddress = marketAddress;
        }
        
        public long TokenId { get; }
        public string MarketAddress { get; }
    }
}