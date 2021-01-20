namespace Opdex.Indexer.Domain.Models.LogEvents
{
    public class BurnEvent : LogEventBase
    {
        public string Sender { get; }
        public string To { get; }
        public ulong AmountCrs { get; }
        public ulong AmountToken { get; }
    }
}