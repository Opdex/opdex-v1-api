using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.LiquidityPools.Snapshots
{
    public class CostSnapshotDto
    {
        public OhlcDto<FixedDecimal> CrsPerSrc { get; set; }
        public OhlcDto<FixedDecimal> SrcPerCrs { get; set; }
    }
}
