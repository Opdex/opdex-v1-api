using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Opdex.Platform.WebApi.Models.Responses.OHLC;

/// <summary>
/// Value details within a time interval.
/// </summary>
public class OhlcDecimalResponseModel
{
    /// <summary>
    /// Value at the start of the time interval.
    /// </summary>
    /// <example>55.00</example>
    [NotNull]
    [Range(0, double.MaxValue)]
    public decimal Open { get; set; }

    /// <summary>
    /// Highest value during the time interval.
    /// </summary>
    /// <example>55.55</example>
    [NotNull]
    [Range(0, double.MaxValue)]
    public decimal High { get; set; }

    /// <summary>
    /// Lowest value during the time interval.
    /// </summary>
    /// <example>49.85</example>
    [NotNull]
    [Range(0, double.MaxValue)]
    public decimal Low { get; set; }

    /// <summary>
    /// Value at the end of the time interval.
    /// </summary>
    /// <example>55.25</example>
    [NotNull]
    [Range(0, double.MaxValue)]
    public decimal Close { get; set; }
}