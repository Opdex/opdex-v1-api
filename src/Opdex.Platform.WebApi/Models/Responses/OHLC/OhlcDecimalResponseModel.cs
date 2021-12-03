using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Opdex.Platform.WebApi.Models.Responses.OHLC;

/// <summary>
/// Price details within a time interval.
/// </summary>
public class OhlcDecimalResponseModel
{
    /// <summary>
    /// Price at the start of the time interval.
    /// </summary>
    /// <example>55.00</example>
    [NotNull]
    [Range(0, double.MaxValue)]
    public decimal Open { get; set; }

    /// <summary>
    /// Highest price during the time interval.
    /// </summary>
    /// <example>55.55</example>
    [NotNull]
    [Range(0, double.MaxValue)]
    public decimal High { get; set; }

    /// <summary>
    /// Lowest price during the time interval.
    /// </summary>
    /// <example>49.85</example>
    [NotNull]
    [Range(0, double.MaxValue)]
    public decimal Low { get; set; }

    /// <summary>
    /// Price at the end of the time interval.
    /// </summary>
    /// <example>55.25</example>
    [NotNull]
    [Range(0, double.MaxValue)]
    public decimal Close { get; set; }
}