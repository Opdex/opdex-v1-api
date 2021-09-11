using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Requests.Quotes
{
    public class AddLiquidityQuoteRequestModel
    {
        /// <summary>
        /// Decimal number as string of the amount of tokens to be deposited into a pool.
        /// </summary>
        public FixedDecimal AmountIn { get; set; }

        /// <summary>
        /// The smart contract address of the deposited token or "CRS" for Cirrus token.
        /// </summary>
        public Address TokenIn { get; set; }

        /// <summary>
        /// The address of the liquidity pool to get a quote for.
        /// </summary>
        public Address Pool { get; set; }
    }
}
