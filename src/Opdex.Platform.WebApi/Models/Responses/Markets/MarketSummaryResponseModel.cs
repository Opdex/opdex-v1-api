using Opdex.Platform.WebApi.Models.Responses.LiquidityPools.Summary;

namespace Opdex.Platform.WebApi.Models.Responses.Markets;

public class MarketSummaryResponseModel
{
    /// <summary>
    /// The total USD of liquidity locked within the market.
    /// </summary>
    /// <example>1986123.12</example>
    public decimal LiquidityUsd { get; set; }

    /// <summary>
    /// The daily percent change of liquidity in USD.
    /// </summary>
    /// <example>-1.12</example>
    public decimal DailyLiquidityUsdChangePercent { get; set; }

    /// <summary>
    /// The total daily transaction volume.
    /// </summary>
    /// <example>50232.23</example>
    public decimal VolumeUsd { get; set; }

    /// <summary>
    /// Summary of staking details for a staking market.
    /// </summary>
    public MarketStakingResponseModel Staking { get; set; }

    /// <summary>
    /// Summary of daily rewards the market and volume have generated.
    /// </summary>
    public RewardsResponseModel Rewards { get; set; }

    /// <summary>
    /// Block number at which the entity state was created.
    /// </summary>
    /// <example>2500000</example>
    public ulong CreatedBlock { get; set; }

    /// <summary>
    /// Block number at which the entity state was last modified.
    /// </summary>
    /// <example>3000000</example>
    public ulong ModifiedBlock { get; set; }
}
