using NJsonSchema.Annotations;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.MarketTokens
{
    public class SwapAmountOutQuoteResponseModel
    {
        /// <summary>
        /// The output amount of tokens after a swap.
        /// </summary>
        [NotNull]
        public FixedDecimal AmountOut { get; set; }
    }
}