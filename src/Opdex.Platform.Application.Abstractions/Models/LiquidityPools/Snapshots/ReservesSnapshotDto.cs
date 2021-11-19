using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.LiquidityPools.Snapshots
{
    public class ReservesSnapshotDto
    {
        public OhlcDto<decimal> Crs { get; set; }
        public OhlcDto<FixedDecimal> Src { get; set; }
        public OhlcDto<decimal> Usd { get; set; }
    }
}
