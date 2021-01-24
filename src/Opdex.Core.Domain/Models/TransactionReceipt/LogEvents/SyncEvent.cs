namespace Opdex.Core.Domain.Models.TransactionReceipt.LogEvents
{
    public class SyncEvent : LogEventBase
    {
        public SyncEvent(dynamic log) : base(nameof(SyncEvent))
        {
            ulong reserveCrs = log?.reserveCrs;
            ulong reserveToken = log?.reserveToken;

            ReserveCrs = reserveCrs;
            ReserveToken = reserveToken;
        }
        
        public ulong ReserveCrs { get; }
        public ulong ReserveToken { get; }
    }
}