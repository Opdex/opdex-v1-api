namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Pools.LiquidityPoolSnapshots
{
    public class SnapshotReservesEntity
    {
        public string ReserveCrs { get; set; }
        public string ReserveSrc { get; set; }
        public decimal ReserveUsd { get; set; }
    }
}