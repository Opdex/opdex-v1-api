using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.LiquidityPools.Snapshots;

public class SnapshotReservesEntity
{
    public OhlcEntity<ulong> Crs { get; set; }
    public OhlcEntity<UInt256> Src { get; set; }
    public OhlcEntity<decimal> Usd { get; set; }
}