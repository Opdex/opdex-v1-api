namespace Opdex.Indexer.Domain.Models.LogEvents
{
    public class SwapEvent : LogEventBase
    {
        public string Sender { get; }
        public string To { get; }
        public ulong AmountCrsIn { get; }
        public ulong AmountTokenIn { get; }
        public ulong AmountCrsOut { get; }
        public ulong AmountTokenOut { get; }
    }
}