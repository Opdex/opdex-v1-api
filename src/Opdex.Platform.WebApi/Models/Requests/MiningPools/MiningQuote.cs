using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Requests.MiningPools;

/// <summary>
/// A request to quote a mining transaction.
/// </summary>
public class MiningQuote
{
    /// <summary>
    /// The amount of liquidity pool tokens to use for the quote.
    /// </summary>
    /// <example>"500.00000000"</example>
    public FixedDecimal Amount { get; set; }
}