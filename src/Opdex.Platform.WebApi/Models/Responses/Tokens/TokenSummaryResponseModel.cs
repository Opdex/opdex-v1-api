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
    public decimal PriceUsd { get; set; }

    /// <summary>
    /// Price change percentage for the current day. This is reset at 00:00 UTC.
    /// </summary>
    /// <example>1.69</example>
    public decimal DailyPriceChangePercent { get; set; }

    /// <summary>
    /// Block number at which the entity was created.
    /// </summary>
    /// <example>2500000</example>
    public ulong CreatedBlock { get; set; }

    /// <summary>
    /// Block number at which the entity state was last modified.
    /// </summary>
    /// <example>3000000</example>
    public ulong ModifiedBlock { get; set; }
}