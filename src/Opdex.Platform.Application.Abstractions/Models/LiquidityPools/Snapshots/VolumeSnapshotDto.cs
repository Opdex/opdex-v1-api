using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.LiquidityPools.Snapshots;

public class VolumeSnapshotDto
{
    public FixedDecimal Crs { get; set; }

    public FixedDecimal Src { get; set; }

    public decimal Usd { get; set; }
}