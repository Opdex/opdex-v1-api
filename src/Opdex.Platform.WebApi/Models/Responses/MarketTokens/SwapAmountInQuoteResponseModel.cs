using NJsonSchema.Annotations;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.MarketTokens
{
    public class SwapAmountInQuoteResponseModel
    {
        /// <summary>
        /// The input amount of tokens for a swap.
        /// </summary>
        [NotNull]
        public FixedDecimal AmountIn { get; set; }
    }
}
