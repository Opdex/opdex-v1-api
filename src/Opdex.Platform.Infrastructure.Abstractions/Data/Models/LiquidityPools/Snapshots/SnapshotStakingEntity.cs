using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.LiquidityPools.Snapshots;

public class SnapshotStakingEntity
{
    public OhlcEntity<UInt256> Weight { get; set; }
    public OhlcEntity<decimal> Usd { get; set; }
}