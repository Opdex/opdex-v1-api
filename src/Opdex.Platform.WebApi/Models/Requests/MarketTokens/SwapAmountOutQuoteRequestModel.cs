using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Requests.MarketTokens;

/// <summary>
/// A request to retrieve the estimated amount of tokens output from a swap.
/// </summary>
public class SwapAmountOutQuoteRequestModel
{
    /// <summary>
    /// The contract address of the token being input, use "CRS" for Cirrus token.
    /// </summary>
    /// <example>tMdZ2UfwJorAyErDvqNdVU8kmiLaykuE5L</example>
    public Address TokenIn { get; set; }

    /// <summary>
    /// The expected amount of tokens to be input for the swap.
    /// </summary>
    /// <example>"0.5000000"</example>
    public FixedDecimal TokenInAmount { get; set; }
}