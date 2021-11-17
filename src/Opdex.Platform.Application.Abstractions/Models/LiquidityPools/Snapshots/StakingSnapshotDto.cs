using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.LiquidityPools.Snapshots
{
    public class StakingSnapshotDto
    {
        public FixedDecimal Weight { get; set; }
        public decimal Usd { get; set; }
    }
}
