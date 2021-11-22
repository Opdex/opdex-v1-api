using Opdex.Platform.Application.Abstractions.Models.LiquidityPools.Snapshots;
using System;

namespace Opdex.Platform.Application.Abstractions.Models.Markets
{
    public class MarketSnapshotDto
    {
        public ulong Id { get; set; }
        public ulong MarketId { get; set; }
        public OhlcDto<decimal> LiquidityUsd { get; set; }
        public decimal VolumeUsd { get; set; }
        public StakingSnapshotDto Staking { get; set; }
        public RewardsSnapshotDto Rewards { get; set; }
        public int SnapshotType { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
