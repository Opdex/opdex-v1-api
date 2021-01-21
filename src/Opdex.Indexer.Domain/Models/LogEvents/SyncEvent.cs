namespace Opdex.Indexer.Domain.Models.LogEvents
{
    public class SyncEvent : LogEventBase
    {
        public SyncEvent()
        {
            
        }
        
        public ulong ReserveCrs { get; }
        public ulong ReserveToken { get; }
    }
}