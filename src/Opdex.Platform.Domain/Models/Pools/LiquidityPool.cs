using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models.Pools
{
    public class LiquidityPool
    {
        public LiquidityPool(string address, long tokenId, long marketId, ulong createdBlock, ulong modifiedBlock)
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
            
            if (createdBlock < 1)
            {
                throw new ArgumentNullException(nameof(createdBlock));
            }
            
            if (modifiedBlock < 1)
            {
                throw new ArgumentNullException(nameof(modifiedBlock));
            }
            
            Address = address;
            TokenId = tokenId;
            MarketId = marketId;
            CreatedBlock = createdBlock;
            ModifiedBlock = modifiedBlock;
        }
        
        public LiquidityPool(string address, string tokenAddress, ulong createdBlock, ulong modifiedBlock)
        {
            if (!address.HasValue())
            {
                throw new ArgumentNullException(nameof(address), $"{nameof(address)} must be provided");
            }
            
            if (!tokenAddress.HasValue())
            {
                throw new ArgumentNullException(nameof(tokenAddress), $"{nameof(tokenAddress)} must be provided");
            }
            
            if (createdBlock < 1)
            {
                throw new ArgumentNullException(nameof(createdBlock));
            }
            
            if (modifiedBlock < 1)
            {
                throw new ArgumentNullException(nameof(modifiedBlock));
            }
            
            Address = address;
            TokenAddress = tokenAddress;
            CreatedBlock = createdBlock;
            ModifiedBlock = modifiedBlock;
        }

        public LiquidityPool(long id, string address, long tokenId, long marketId, ulong createdBlock, ulong modifiedBlock)
        {
            Id = id;
            Address = address;
            TokenId = tokenId;
            MarketId = marketId;
            CreatedBlock = createdBlock;
            ModifiedBlock = modifiedBlock;
        }
        
        public long Id { get; }
        public string Address { get; }
        public long TokenId { get; private set; }
        public long MarketId { get; private set; }
        public string TokenAddress { get; }
        public ulong CreatedBlock { get; }
        public ulong ModifiedBlock { get; }

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