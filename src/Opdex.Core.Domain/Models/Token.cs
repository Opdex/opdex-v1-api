using System;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models
{
    public class Token
    {
        public Token(string address, string name, string symbol, int decimals, long sats, ulong totalSupply)
        {
            if (!address.HasValue())
            {
                throw new ArgumentNullException(nameof(address));
            }
            
            if (!name.HasValue())
            {
                throw new ArgumentNullException(nameof(name));
            }
            
            if (!symbol.HasValue())
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            if (decimals < 0 || decimals > 18)
            {
                throw new ArgumentOutOfRangeException(nameof(decimals));
            }

            if (sats < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(sats));
            }

            if (totalSupply < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(totalSupply));
            }

            Address = address;
            Name = name;
            Symbol = symbol;
            Decimals = decimals;
            Sats = sats;
            TotalSupply = totalSupply;
        }

        protected internal Token(long id, string address, string name, string symbol, int decimals, long sats, ulong totalSupply)
        {
            Id = id;
            Address = address;
            Name = name;
            Symbol = symbol;
            Decimals = decimals;
            Sats = sats;
            TotalSupply = totalSupply;
        }
        
        public long Id { get; }
        public string Address { get; }
        public string Name { get; }
        public string Symbol { get; }
        public int Decimals { get; }
        public long Sats { get; }
        public ulong TotalSupply { get; }
    }
}