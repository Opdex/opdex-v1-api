using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Blocks;
using System;

namespace Opdex.Platform.Domain.Models.Tokens
{
    public abstract class BaseToken : BlockAudit
    {
        protected BaseToken(Address address, bool isLpt, string name, string symbol, int decimals, ulong sats,
                         UInt256 totalSupply, ulong createdBlock) : base(createdBlock)
        {
            if (address == Address.Empty)
            {
                throw new ArgumentNullException(nameof(address), "Token address must be set.");
            }

            if (!name.HasValue())
            {
                throw new ArgumentNullException(nameof(name), "Token name must be set.");
            }

            if (!symbol.HasValue())
            {
                throw new ArgumentNullException(nameof(symbol), "Token symbol must be set.");
            }

            if (decimals < 0 || decimals > 18)
            {
                throw new ArgumentOutOfRangeException(nameof(decimals), "Token must have between 0 and 18 decimal denominations.");
            }

            if (sats < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(sats), "Sats must be greater than zero.");
            }

            Address = address;
            IsLpt = isLpt;
            Name = name;
            Symbol = symbol;
            Decimals = decimals;
            Sats = sats;
            TotalSupply = totalSupply;
        }

        protected BaseToken(Address address, bool isLpt, string name, string symbol, int decimals, ulong sats,
                            UInt256 totalSupply, ulong createdBlock, ulong modifiedBlock) : base(createdBlock, modifiedBlock)
        {
            Address = address;
            IsLpt = isLpt;
            Name = name;
            Symbol = symbol;
            Decimals = decimals;
            Sats = sats;
            TotalSupply = totalSupply;
        }

        protected BaseToken(ulong createdBlock) : base(createdBlock)
        {
        }

        public Address Address { get; }
        public bool IsLpt { get; }
        public string Name { get; }
        public string Symbol { get; }
        public int Decimals { get; }
        public ulong Sats { get; }
        public UInt256 TotalSupply { get; protected set; }
    }
}
