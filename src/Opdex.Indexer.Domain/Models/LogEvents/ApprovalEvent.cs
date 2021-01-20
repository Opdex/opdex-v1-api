namespace Opdex.Indexer.Domain.Models.LogEvents
{
    public class ApprovalEvent : LogEventBase
    {
        public string Owner { get; }
        public string Spender { get; }
        public ulong Amount { get; }
    }
}