namespace Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionEvents
{
    public class SyncEventEntity : EventEntityBase
    {
        public ulong ReserveCrs { get; set; }
        public string ReserveSrc { get; set; }
    }
}