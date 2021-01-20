namespace Opdex.Indexer.Domain.Models.LogEvents
{
    public class PairCreatedEvent : LogEventBase
    {
        public string Token { get; }
        public string Pair { get; }
    }
}