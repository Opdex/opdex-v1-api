using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using System;

namespace Opdex.Platform.Domain.Models.Tokens
{
    public class TokenContractSummary
    {
        public TokenContractSummary(Address address, string name, string symbol, uint decimals, UInt256 totalSupply)
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

            if (decimals > 18)
            {
                throw new ArgumentOutOfRangeException(nameof(decimals), "Token must have between 0 and 18 decimal denominations.");
            }

            Address = address;
            Name = name;
            Symbol = symbol;
            Decimals = decimals;
            TotalSupply = totalSupply;
        }

        public Address Address { get; }
        public string Name { get; }
        public string Symbol { get; private set; }
        public uint Decimals { get; }
        public ulong Sats => SatoshiConverterExtension.SatsFromPrecision((int)Decimals);
        public UInt256 TotalSupply { get; }
    }
}
