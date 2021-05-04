using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models
{
    public class LiquidityPool
    {
        public LiquidityPool(string address, long tokenId, long marketId)
        {
            if (!address.HasValue())
            {
                throw new ArgumentNullException(nameof(address), $"{nameof(address)} must be provided");
            }
            
            if (tokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenId), $"{nameof(tokenId)} must be greater than 0.");
            }
            
            if (marketId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(marketId), $"{nameof(marketId)} must be greater than 0.");
            }
            
            Address = address;
            TokenId = tokenId;
            MarketId = marketId;
        }
        
        public LiquidityPool(string address, string tokenAddress)
        {
            if (!address.HasValue())
            {
                throw new ArgumentNullException(nameof(address), $"{nameof(address)} must be provided");
            }
            
            if (!tokenAddress.HasValue())
            {
                throw new ArgumentNullException(nameof(tokenAddress), $"{nameof(tokenAddress)} must be provided");
            }
            
            Address = address;
            TokenAddress = tokenAddress;
        }

        public LiquidityPool(long id, string address, long tokenId, long marketId)
        {
            Id = id;
            Address = address;
            TokenId = tokenId;
            MarketId = marketId;
        }
        
        public long Id { get; }
        public string Address { get; }
        public long TokenId { get; private set; }
        public long MarketId { get; private set; }
        public string TokenAddress { get; }

        public void SetTokenId(long tokenId)
        {
            if (TokenId == 0 && tokenId > 0)
            {
                TokenId = tokenId;
            }
        }
        
        public void SetMarketId(long marketId)
        {
            if (MarketId == 0 && marketId > 0)
            {
                MarketId = marketId;
            }
        }
    }
}