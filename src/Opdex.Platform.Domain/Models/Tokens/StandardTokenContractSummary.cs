using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models.UInt;
using System;

namespace Opdex.Platform.Domain.Models.Tokens
{
    public class StandardTokenContractSummary
    {
        public StandardTokenContractSummary(ulong blockHeight)
        {
            if (blockHeight == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
            }

            BlockHeight = blockHeight;
        }

        public ulong BlockHeight { get; }
        public string Name { get; private set; }
        public string Symbol { get; private set; }
        public uint? Decimals { get; private set; }
        public ulong? Sats { get; private set; }
        public bool? IsLpt { get; private set; }
        public UInt256? TotalSupply { get; private set; }

        public void SetBaseProperties(string name, string symbol, uint decimals)
        {
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

            Name = name;
            Symbol = symbol;
            Decimals = decimals;
            Sats = SatoshiConverterExtension.SatsFromPrecision((int)Decimals.Value);
            IsLpt = Name == TokenConstants.LiquidityPoolToken.Name && Symbol == TokenConstants.LiquidityPoolToken.Symbol;
        }

        public void SetTotalSupply(UInt256 supply)
        {
            TotalSupply = supply;
        }
    }
}
