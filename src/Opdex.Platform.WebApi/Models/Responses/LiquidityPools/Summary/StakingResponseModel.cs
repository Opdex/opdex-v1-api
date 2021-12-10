using NJsonSchema.Annotations;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Responses.MarketTokens;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Responses.LiquidityPools.Summary;

/// <summary>
/// Staking summary.
/// </summary>
public class StakingResponseModel
{
    /// <summary>
    /// The governance token used for staking.
    /// </summary>
    [NotNull]
    public MarketTokenResponseModel Token { get; set; }

    /// <summary>
    /// Total number of tokens staking.
    /// </summary>
    /// <example>100000.0000000</example>
    [NotNull]
    public FixedDecimal Weight { get; set; }

    /// <summary>
    /// Total USD value of tokens staking.
    /// </summary>
    /// <example>425000.50</example>
    [NotNull]
    [Range(0, double.MaxValue)]
    public decimal Usd { get; set; }

    /// <summary>
    /// Percentage change in staking weight for the day.
    /// </summary>
    /// <example>-4.69</example>
    [NotNull]
    [Range(double.MinValue, double.MaxValue)]
    public decimal DailyWeightChangePercent { get; set; }

    /// <summary>
    /// Flag determining if the liquidity pool is nominated for mining.
    /// </summary>
    /// <example>true</example>
    [NotNull]
    public bool Nominated { get; set; }
}