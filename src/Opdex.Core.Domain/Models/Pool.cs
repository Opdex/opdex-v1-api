using System;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models
{
    public class Pool
    {
        public Pool(string address, long tokenId, ulong reserveCrs, string reserveSrc)
        {
            if (!address.HasValue())
            {
                throw new ArgumentNullException(nameof(address), $"{nameof(address)} must be provided");
            }
            
            if (tokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenId), $"{nameof(tokenId)} must be greater than 0.");
            }

            if (!reserveSrc.HasValue())
            {
                throw new ArgumentNullException(nameof(reserveSrc));
            }
            
            Address = address;
            TokenId = tokenId;
            ReserveCrs = reserveCrs;
            ReserveSrc = reserveSrc;
        }
        
        public Pool(string address, string tokenAddress, ulong reserveCrs, string reserveSrc)
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

        public Pool(long id, string address, long tokenId, ulong reserveCrs, string reserveSrc)
        {
            Id = id;
            Address = address;
            TokenId = tokenId;
            ReserveCrs = reserveCrs;
            ReserveSrc = reserveSrc;
        }
        
        public long Id { get; }
        public string Address { get; }
        public long TokenId { get; private set; }
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