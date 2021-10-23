using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Requests.MarketTokens
{
    public class SwapAmountOutQuoteRequestModel
    {
        /// <summary>
        /// The contract address of the token being input, use "CRS" for Cirrus token.
        /// </summary>
        public Address TokenIn { get; set; }

        /// <summary>
        /// The expected amount of tokens to be input for the swap.
        /// </summary>
        public FixedDecimal TokenInAmount { get; set; }
    }
}
