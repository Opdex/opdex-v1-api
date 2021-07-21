namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Tokens
{
    public class DistributionEventDto : TransactionEventDto
    {
        public string VaultAmount { get; set; }
        public string GovernanceAmount { get; set; }
        public uint PeriodIndex { get; set; }
        public override TransactionEventType EventType => TransactionEventType.DistributionEvent;
    }
}
