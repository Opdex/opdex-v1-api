using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.LiquidityPools.Snapshots
{
    public class SnapshotStakingEntity
    {
        public UInt256 Weight { get; set; }
        public decimal Usd { get; set; }
    }
}
