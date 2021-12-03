using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Responses.Tokens;

/// <summary>
/// Point in time snapshot summary for a token.
/// </summary>
public class TokenSummaryResponseModel
{
    /// <summary>
    /// Calculated token price in USD.
    /// </summary>
    /// <example>"4.25"</example>
    [Range(0, double.MaxValue)]
    public decimal PriceUsd { get; set; }

    /// <summary>
    /// Price change percentage for the current day. This is reset at 00:00 UTC.
    /// </summary>
    /// <example>1.69</example>
    public decimal DailyPriceChangePercent { get; set; }

    /// <summary>
    /// The block which the token was last updated.
    /// </summary>
    /// <example>500000</example>
    [Range(1, double.MaxValue)]
    public ulong ModifiedBlock { get; set; }
}