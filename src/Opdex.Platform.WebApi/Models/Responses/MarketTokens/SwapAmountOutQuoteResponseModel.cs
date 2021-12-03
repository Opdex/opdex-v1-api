using NJsonSchema.Annotations;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.MarketTokens;

/// <summary>
/// Details the estimated amount of tokens output for a given swap input.
/// </summary>
public class SwapAmountOutQuoteResponseModel
{
    /// <summary>
    /// The output amount of tokens after a swap.
    /// </summary>
    /// <example>"10000.00000000"</example>
    [NotNull]
    public FixedDecimal AmountOut { get; set; }
}