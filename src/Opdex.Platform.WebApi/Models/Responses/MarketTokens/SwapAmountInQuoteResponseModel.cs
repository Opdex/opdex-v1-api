using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.MarketTokens;

/// <summary>
/// Details the estimated amount of input tokens required for a given swap output.
/// </summary>
public class SwapAmountInQuoteResponseModel
{
    /// <summary>
    /// The input amount of tokens for a swap.
    /// </summary>
    /// <example>"10.00000000"</example>
    public FixedDecimal AmountIn { get; set; }
}