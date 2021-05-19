using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models.Tokens
{
    public class Token : BlockAudit
    {
        public Token(string address, string name, string symbol, int decimals, ulong sats, string totalSupply, ulong createdBlock)
            : base(createdBlock)
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

            if (!totalSupply.HasValue())
            {
                throw new ArgumentNullException(nameof(totalSupply));
            }

            Address = address;
            Name = name;
            Symbol = symbol;
            Decimals = decimals;
            Sats = sats;
            TotalSupply = totalSupply;
        }

        public Token(long id, string address, string name, string symbol, int decimals, ulong sats, string totalSupply, ulong createdBlock, ulong modifiedBlock)
            : base(createdBlock, modifiedBlock)
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
        public ulong Sats { get; }
        public string TotalSupply { get; }
    }
}