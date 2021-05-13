using System;
using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pools
{
    public class SelectLiquidityPoolByTokenIdAndMarketAddressQuery : IRequest<Token>
    {
        public SelectLiquidityPoolByTokenIdAndMarketAddressQuery(long tokenId, string marketAddress)
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