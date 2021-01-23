namespace Opdex.Indexer.Domain.Models.LogEvents
{
    public class TransferEvent : LogEventBase
    {
        public TransferEvent()
        {
            
        }
        
        public string From { get; }
        public string To { get; }
        public ulong Amount { get; }
    }
}