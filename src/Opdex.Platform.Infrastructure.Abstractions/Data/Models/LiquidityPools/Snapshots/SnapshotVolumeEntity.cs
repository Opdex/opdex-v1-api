using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.LiquidityPools.Snapshots
{
    public class SnapshotVolumeEntity
    {
        public ulong Crs { get; set; }
        public UInt256 Src { get; set; }
        public decimal Usd { get; set; }
    }
}
