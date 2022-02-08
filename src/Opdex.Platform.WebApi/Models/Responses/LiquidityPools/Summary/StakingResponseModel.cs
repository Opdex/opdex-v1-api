using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Responses.MarketTokens;

namespace Opdex.Platform.WebApi.Models.Responses.LiquidityPools.Summary;

/// <summary>
/// Staking summary.
/// </summary>
public class StakingResponseModel
{
    /// <summary>
    /// The governance token used for staking.
    /// </summary>
    public MarketTokenResponseModel Token { get; set; }

    /// <summary>
    /// Total number of tokens staking.
    /// </summary>
    /// <example>100000.0000000</example>
    public FixedDecimal Weight { get; set; }

    /// <summary>
    /// Total USD value of tokens staking.
    /// </summary>
    /// <example>425000.50</example>
    public decimal Usd { get; set; }

    /// <summary>
    /// Percentage change in staking weight for the day.
    /// </summary>
    /// <example>-4.69</example>
    public decimal DailyWeightChangePercent { get; set; }

    /// <summary>
    /// Flag determining if the liquidity pool is nominated for mining.
    /// </summary>
    /// <example>true</example>
    public bool Nominated { get; set; }
}
