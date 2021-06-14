namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Pools.LiquidityPoolSnapshots
{
    public class SnapshotVolumeEntity
    {
        public string VolumeCrs { get; set; }
        public string VolumeSrc { get; set; }
        public decimal VolumeUsd { get; set; }
    }
}