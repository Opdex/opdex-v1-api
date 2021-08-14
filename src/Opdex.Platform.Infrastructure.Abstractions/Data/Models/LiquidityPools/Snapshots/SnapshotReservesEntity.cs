namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.LiquidityPools.Snapshots
{
    public class SnapshotReservesEntity
    {
        public ulong Crs { get; set; }
        public string Src { get; set; }
        public decimal Usd { get; set; }
    }
}
