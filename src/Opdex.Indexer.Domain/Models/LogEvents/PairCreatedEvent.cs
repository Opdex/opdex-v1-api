namespace Opdex.Indexer.Domain.Models.LogEvents
{
    public class PairCreatedEvent : LogEventBase
    {
        public PairCreatedEvent()
        {
            
        }
        
        public string Token { get; }
        public string Pair { get; }
    }
}