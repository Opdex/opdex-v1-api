using NJsonSchema.Annotations;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Pools
{
    public class AddLiquidityAmountInQuoteResponseModel
    {
        /// <summary>
        /// The quoted amount of tokens to provide to match the requested amount.
        /// </summary>
        [NotNull]
        public FixedDecimal AmountIn { get; set; }
    }
}
