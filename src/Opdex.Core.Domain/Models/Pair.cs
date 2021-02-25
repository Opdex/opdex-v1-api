using System;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models
{
    public class Pair
    {
        public Pair (string address, long tokenId, ulong reserveCrs, string reserveSrc)
        {
            if (!address.HasValue())
            {
                throw new ArgumentNullException(nameof(address), $"{nameof(address)} must be provided");
            }
            
            if (tokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenId), $"{nameof(tokenId)} must be greater than 0.");
            }
            
            if (reserveCrs < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(reserveCrs), $"{nameof(reserveCrs)} must be equal or greater than 0.");
            }
            
            if (!reserveSrc.HasValue())
            {
                throw new ArgumentNullException(nameof(reserveSrc));
            }
            
            Address = address;
            TokenId = tokenId;
            ReserveCrs = reserveCrs;
            ReserveToken = reserveSrc;
        }
        
        public long Id { get; }
        public string Address { get; }
        public long TokenId { get; }
        public ulong ReserveCrs { get; }
        public string ReserveToken { get; }
    }
}