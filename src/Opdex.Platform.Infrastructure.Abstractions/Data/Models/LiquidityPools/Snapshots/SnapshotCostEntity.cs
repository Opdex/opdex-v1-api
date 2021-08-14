using Opdex.Platform.Infrastructure.Abstractions.Data.Models.OHLC;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.LiquidityPools.Snapshots
{
    public class SnapshotCostEntity
    {
        public OhlcBigIntEntity CrsPerSrc { get; set; }
        public OhlcBigIntEntity SrcPerCrs { get; set; }
    }
}
