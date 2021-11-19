using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.LiquidityPools.Snapshots
{
    public class StakingSnapshotDto
    {
        public OhlcDto<FixedDecimal> Weight { get; set; }
        public OhlcDto<decimal> Usd { get; set; }
    }
}
