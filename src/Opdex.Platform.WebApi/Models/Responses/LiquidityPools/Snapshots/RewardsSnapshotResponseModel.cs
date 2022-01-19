
namespace Opdex.Platform.WebApi.Models.Responses.LiquidityPools.Snapshots;

/// <summary>
/// Reward snapshot details.
/// </summary>
public class RewardsSnapshotResponseModel
{
    /// <summary>
    /// USD value of rewards to providers.
    /// </summary>
    /// <example>50000.50</example>
    public decimal ProviderUsd { get; set; }

    /// <summary>
    /// USD value of rewards to the market, either the owner of a standard market or stakers in a staking market.
    /// </summary>
    /// <example>10000.10</example>
    public decimal MarketUsd { get; set; }

    /// <summary>
    /// USD value of all rewards generated from token swaps.
    /// </summary>
    /// <example>60000.60</example>
    public decimal TotalUsd { get; set; }
}