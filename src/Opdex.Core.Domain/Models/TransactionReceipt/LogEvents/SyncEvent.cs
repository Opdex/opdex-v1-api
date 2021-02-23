namespace Opdex.Core.Domain.Models.TransactionReceipt.LogEvents
{
    public class SyncEvent : LogEventBase
    {
        public SyncEvent(dynamic log) : base(nameof(SyncEvent))
        {
            ulong reserveCrs = log?.ReserveCrs;
            string reserveSrc = log?.ReserveSrc;

            ReserveCrs = reserveCrs;
            ReserveSrc = reserveSrc;
        }
        
        public ulong ReserveCrs { get; }
        public string ReserveSrc { get; }
    }
}