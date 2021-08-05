using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Domain.Models.Tokens
{
    public class TokenContractSummary
    {
        public TokenContractSummary(string address, string name, string symbol, uint decimals, string totalSupply)
        {
            if (!address.HasValue())
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

            if (!totalSupply.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(totalSupply), "Total supply must only contain numeric digits.");
            }

            Address = address;
            Name = name;
            Symbol = symbol;
            Decimals = decimals;
            TotalSupply = totalSupply;
        }

        public string Address { get; }
        public string Name { get; }
        public string Symbol { get; private set; }
        public uint Decimals { get; }
        public ulong Sats => Decimals.DecimalsToSatoshis();
        public string TotalSupply { get; }

        public void SetLpTokenSymbol(string symbol)
        {
            if (Name != TokenConstants.LiquidityPoolToken.Name || Symbol != TokenConstants.LiquidityPoolToken.Symbol)
            {
                throw new Exception("The token must be a liquidity pool token to change it's ticker symbol.");
            }

            this.Symbol = symbol;
        }
    }
}
