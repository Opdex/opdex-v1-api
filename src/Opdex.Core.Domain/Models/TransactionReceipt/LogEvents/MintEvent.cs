namespace Opdex.Indexer.Domain.Models.LogEvents
{
    public class MintEvent : LogEventBase
    {
        public MintEvent()
        {
            
        }
        
        public string Sender { get; }
        public ulong AmountCrs { get; }
        public ulong AmountToken { get; }
    }
}