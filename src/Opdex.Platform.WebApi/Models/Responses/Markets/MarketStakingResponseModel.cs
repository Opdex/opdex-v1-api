using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Markets;

/// <summary>
/// Summary of staking details for a market.
/// </summary>
public class MarketStakingResponseModel
{
    /// <summary>
    /// The number of tokens staked within the market.
    /// </summary>
    /// <example>"272311.78654323"</example>
    public FixedDecimal StakingWeight { get; set; }

    /// <summary>
    /// The daily percentage change of the number of staked tokens within the market.
    /// </summary>
    /// <example>2.23</example>
    public decimal DailyStakingWeightChangePercent { get; set; }

    /// <summary>
    /// The USD amount of tokens staked within the market.
    /// </summary>
    /// <example>45123.54</example>
    public decimal StakingUsd { get; set; }

    /// <summary>
    /// The daily percentage change of staking USD amounts.
    /// </summary>
    /// <example>1.12</example>
    public decimal DailyStakingUsdChangePercent { get; set; }
}
