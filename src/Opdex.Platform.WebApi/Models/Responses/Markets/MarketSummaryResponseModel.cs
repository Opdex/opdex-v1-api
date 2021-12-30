using NJsonSchema.Annotations;
using Opdex.Platform.WebApi.Models.Responses.LiquidityPools.Summary;

namespace Opdex.Platform.WebApi.Models.Responses.Markets;

public class MarketSummaryResponseModel
{
    /// <summary>
    /// The total USD of liquidity locked within the market.
    /// </summary>
    /// <example>1986123.12</example>
    [NotNull]
    public decimal LiquidityUsd { get; set; }

    /// <summary>
    /// The daily percent change of liquidity in USD.
    /// </summary>
    /// <example>-1.12</example>
    [NotNull]
    public decimal DailyLiquidityUsdChangePercent { get; set; }

    /// <summary>
    /// The total daily transaction volume.
    /// </summary>
    /// <example>50232.23</example>
    [NotNull]
    public decimal VolumeUsd { get; set; }

    /// <summary>
    /// Summary of staking details for a staking market.
    /// </summary>
    public MarketStakingResponseModel Staking { get; set; }

    /// <summary>
    /// Summary of daily rewards the market and volume have generated.
    /// </summary>
    [NotNull]
    public RewardsResponseModel Rewards { get; set; }
}
