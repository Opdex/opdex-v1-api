namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Pools.LiquidityPoolSnapshots
{
    public class SnapshotCostEntity
    {
        public OhlcEntity CrsPerSrc { get; set; }
        public OhlcEntity SrcPerCrs { get; set; }
    }
}