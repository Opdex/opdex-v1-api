using System;

namespace Opdex.Platform.Application.Abstractions.Models.LiquidityPools
{
    public class LiquidityPoolSnapshotDto
    {
        public ulong Id { get; set; }
        public ulong LiquidityPoolId { get; set; }
        public long TransactionCount { get; set; }
        public ReservesDto Reserves { get; set; }
        public RewardsDto Rewards { get; set; }
        public StakingDto Staking { get; set; }
        public VolumeDto Volume { get; set; }
        public CostDto Cost { get; set; }
        public int SnapshotTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int SrcTokenDecimals { get; set; }
    }
}
