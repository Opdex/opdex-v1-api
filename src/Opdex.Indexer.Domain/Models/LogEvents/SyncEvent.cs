namespace Opdex.Indexer.Domain.Models.LogEvents
{
    public class SyncEvent : LogEventBase
    {
        public ulong ReserveCrs { get; }
        public ulong ReserveToken { get; }
    }
}