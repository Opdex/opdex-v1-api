using NJsonSchema.Annotations;
using System;

namespace Opdex.Platform.WebApi.Models.Responses.LiquidityPools.Snapshots
{
    public class LiquidityPoolSnapshotResponseModel
    {
        /// <summary>
        /// The start time for the snapshot.
        /// </summary>
        [NotNull]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The number of transactions within the snapshot time period.
        /// </summary>
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
}
