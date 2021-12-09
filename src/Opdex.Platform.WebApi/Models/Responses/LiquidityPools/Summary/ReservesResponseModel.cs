using NJsonSchema.Annotations;
using Opdex.Platform.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Responses.LiquidityPools.Summary;

/// <summary>
/// Reserves summary.
/// </summary>
public class ReservesResponseModel
{
    /// <summary>
    /// Total amount of locked CRS tokens.
    /// </summary>
    /// <example>"100000.00000000"</example>
    [NotNull]
    public FixedDecimal Crs { get; set; }

    /// <summary>
    /// Total amount of locked SRC tokens.
    /// </summary>
    /// <example>"50.00000000"</example>
    [NotNull]
    public FixedDecimal Src { get; set; }

    /// <summary>
    /// Total USD value of locked reserves.
    /// </summary>
    /// <example>"50000000000000.00000000"</example>
    [NotNull]
    [Range(0, double.MaxValue)]
    public decimal Usd { get; set; }

    /// <summary>
    /// Percentage change of liquidity for the day.
    /// </summary>
    /// <example>4.69</example>
    [NotNull]
    [Range(double.MinValue, double.MaxValue)]
    public decimal DailyUsdChangePercent { get; set; }
}