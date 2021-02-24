using System;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models
{
    public class Pair
    {
        public Pair (string address, long tokenId, decimal reserveCrs, decimal reserveToken)
        {
            if (!address.HasValue())
            {
                throw new ArgumentNullException(nameof(address), $"{nameof(address)} must be provided");
            }
            
            if (tokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenId), $"{nameof(tokenId)} must be greater than 0.");
            }
            
            if (reserveCrs < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(reserveCrs), $"{nameof(reserveCrs)} must be equal or greater than 0.");
            }
            
            if (reserveToken < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(reserveToken), $"{nameof(reserveToken)} must be equal or greater than 0.");
            }
            
            Address = address;
            TokenId = tokenId;
            ReserveCrs = reserveCrs;
            ReserveToken = reserveToken;
        }
        
        public long Id { get; }
        public string Address { get; }
        public long TokenId { get; }
        public decimal ReserveCrs { get; }
        public decimal ReserveToken { get; }
    }
}