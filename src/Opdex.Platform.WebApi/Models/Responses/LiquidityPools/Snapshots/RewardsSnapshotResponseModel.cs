using NJsonSchema.Annotations;

namespace Opdex.Platform.WebApi.Models.Responses.LiquidityPools.Snapshots;

public class RewardsSnapshotResponseModel
{
    /// <summary>
    /// The amount of USD rewards to providers.
    /// </summary>
    [NotNull]
    public decimal ProviderUsd { get; set; }

    /// <summary>
    /// The amount of USD rewards to the market, either the market owner of a standard market or stakers of a staking market.
    /// </summary>
    [NotNull]
    public decimal MarketUsd { get; set; }

    /// <summary>
    /// The total amount of USD rewards generated from token swaps.
    /// </summary>
    [NotNull]
    public decimal TotalUsd { get; set; }
}