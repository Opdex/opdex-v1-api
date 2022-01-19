using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.LiquidityPools;

public class AddLiquidityAmountInQuoteResponseModel
{
    /// <summary>
    /// The quoted amount of tokens to provide to match the requested amount.
    /// </summary>
    /// <example>"2500.00000000"</example>
    public FixedDecimal AmountIn { get; set; }
}