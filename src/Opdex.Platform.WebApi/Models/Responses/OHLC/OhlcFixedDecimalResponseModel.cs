using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.OHLC;

/// <summary>
/// Value details within a time interval.
/// </summary>
public class OhlcFixedDecimalResponseModel
{
    /// <summary>
    /// Value at the start of the time interval.
    /// </summary>
    /// <example>10000.00000000</example>
    public FixedDecimal Open { get; set; }

    /// <summary>
    /// Highest value during the time interval.
    /// </summary>
    /// <example>10101.01010101</example>
    public FixedDecimal High { get; set; }

    /// <summary>
    /// Lowest value during the time interval.
    /// </summary>
    /// <example>9876.54321000</example>
    public FixedDecimal Low { get; set; }

    /// <summary>
    /// Value at the end of the time interval.
    /// </summary>
    /// <example>10001.00010001</example>
    public FixedDecimal Close { get; set; }
}