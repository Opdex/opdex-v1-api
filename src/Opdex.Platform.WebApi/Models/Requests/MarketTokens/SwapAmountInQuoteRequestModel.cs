using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Requests.MarketTokens
{
    /// <summary>
    /// A request to retrieve the estimated amount of input tokens required for a swap.
    /// </summary>
    public class SwapAmountInQuoteRequestModel
    {
        /// <summary>
        /// The contract address of the token being output, use "CRS" for Cirrus token.
        /// </summary>
        /// <example>CRS</example>
        public Address TokenOut { get; set; }

        /// <summary>
        /// The expected amount of tokens to be output after the swap.
        /// </summary>
        /// <example>50.00000000</example>
        public FixedDecimal TokenOutAmount { get; set; }
    }
}
