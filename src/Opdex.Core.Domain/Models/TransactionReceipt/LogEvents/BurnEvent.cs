namespace Opdex.Core.Domain.Models.TransactionReceipt.LogEvents
{
    public class BurnEvent : LogEventBase
    {
        public BurnEvent(dynamic log) : base(nameof(BurnEvent))
        {
            
        }
        
        public string Sender { get; }
        public string To { get; }
        public ulong AmountCrs { get; }
        public string AmountSrc { get; }
    }
}