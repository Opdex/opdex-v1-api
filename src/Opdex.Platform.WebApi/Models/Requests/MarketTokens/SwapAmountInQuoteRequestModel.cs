using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Requests.MarketTokens
{
    public class SwapAmountInQuoteRequestModel
    {
        /// <summary>
        /// The contract address of the token being output, use "CRS" for Cirrus token.
        /// </summary>
        public Address TokenOut { get; set; }

        /// <summary>
        /// The expected amount of tokens to be output after the swap.
        /// </summary>
        public FixedDecimal TokenOutAmount { get; set; }
    }
}
