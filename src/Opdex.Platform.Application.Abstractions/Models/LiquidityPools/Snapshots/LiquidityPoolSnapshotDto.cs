using System;

namespace Opdex.Platform.Application.Abstractions.Models.LiquidityPools.Snapshots
{
    public class LiquidityPoolSnapshotDto
    {
        public ulong Id { get; set; }
        public ulong LiquidityPoolId { get; set; }
        public long TransactionCount { get; set; }
        public ReservesSnapshotDto Reserves { get; set; }
        public RewardsSnapshotDto Rewards { get; set; }
        public StakingSnapshotDto Staking { get; set; }
        public VolumeSnapshotDto Volume { get; set; }
        public CostSnapshotDto Cost { get; set; }
        public int SnapshotTypeId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
