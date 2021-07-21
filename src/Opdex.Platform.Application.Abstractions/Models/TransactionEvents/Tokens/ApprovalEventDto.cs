namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Tokens
{
    public class ApprovalEventDto : TransactionEventDto
    {
        public string Owner { get; set; }
        public string Spender { get; set; }
        public string Amount { get; set; }
        public override TransactionEventType EventType => TransactionEventType.ApprovalEvent;
    }
}
