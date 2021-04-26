using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models
{
    public class LiquidityPool
    {
        public LiquidityPool(string address, long tokenId, long marketId, ulong reserveCrs, string reserveSrc)
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

            if (!reserveSrc.HasValue())
            {
                throw new ArgumentNullException(nameof(reserveSrc));
            }
            
            Address = address;
            TokenId = tokenId;
            MarketId = marketId;
            ReserveCrs = reserveCrs;
            ReserveSrc = reserveSrc;
        }
        
        public LiquidityPool(string address, string tokenAddress, ulong reserveCrs, string reserveSrc)
        {
            if (!address.HasValue())
            {
                throw new ArgumentNullException(nameof(address), $"{nameof(address)} must be provided");
            }
            
            if (!tokenAddress.HasValue())
            {
                throw new ArgumentNullException(nameof(tokenAddress), $"{nameof(tokenAddress)} must be provided");
            }
            
            if (!reserveSrc.HasValue())
            {
                throw new ArgumentNullException(nameof(reserveSrc));
            }
            
            Address = address;
            TokenAddress = tokenAddress;
            ReserveCrs = reserveCrs;
            ReserveSrc = reserveSrc;
        }

        public LiquidityPool(long id, string address, long tokenId, long marketId, ulong reserveCrs, string reserveSrc)
        {
            Id = id;
            Address = address;
            TokenId = tokenId;
            MarketId = marketId;
            ReserveCrs = reserveCrs;
            ReserveSrc = reserveSrc;
        }
        
        public long Id { get; }
        public string Address { get; }
        public long TokenId { get; private set; }
        public long MarketId { get; private set; }
        public string TokenAddress { get; }
        public ulong ReserveCrs { get; }
        public string ReserveSrc { get; }

        public void SetTokenId(long tokenId)
        {
            if (TokenId == 0 && tokenId > 0)
            {
                TokenId = tokenId;
            }
        }
    }
}