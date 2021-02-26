namespace Opdex.Core.Application.Abstractions.Models.TransactionEvents
{
    public class ApprovalEventDto : TransactionEventDto
    {
        public string Owner { get; set; }
        public string Spender { get; set; }
        public string Amount { get; set; }
    }
}