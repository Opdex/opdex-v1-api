using NJsonSchema.Annotations;
using System;

namespace Opdex.Platform.WebApi.Models.Responses.LiquidityPools.Snapshots;

/// <summary>
/// Liquidity pool snapshot details.
/// </summary>
public class LiquidityPoolSnapshotResponseModel
{
    /// <summary>
    /// Start time for the snapshot.
    /// </summary>
    /// <example>2022-01-01T00:00:00Z</example>
    [NotNull]
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Number of transactions within the snapshot time period.
    /// </summary>
    /// <example>500</example>
    [NotNull]
    public long TransactionCount { get; set; }

    /// <summary>
    /// Locked reserves details based on snapshot time period.
    /// </summary>
    [NotNull]
    public ReservesSnapshotResponseModel Reserves { get; set; }

    /// <summary>
    /// Rewards details based on snapshot time period.
    /// </summary>
    [NotNull]
    public RewardsSnapshotResponseModel Rewards { get; set; }

    /// <summary>
    /// Volume details based on snapshot time period.
    /// </summary>
    [NotNull]
    public VolumeSnapshotResponseModel Volume { get; set; }

    /// <summary>
    /// Token costs between CRS and SRC reserves for the snapshot time period.
    /// </summary>
    [NotNull]
    public CostSnapshotResponseModel Cost { get; set; }

    /// <summary>
    /// Staking details and weight based on the snapshot time period.
    /// </summary>
    public StakingSnapshotResponseModel Staking { get; set; }
}