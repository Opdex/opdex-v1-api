using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.LiquidityPools.Snapshots;

public class SnapshotCostEntity
{
    public OhlcEntity<UInt256> CrsPerSrc { get; set; }
    public OhlcEntity<UInt256> SrcPerCrs { get; set; }
}