namespace Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionEvents
{
    public class ApprovalEventEntity : EventEntityBase
    {
        public string Owner { get; set; }
        public string Spender { get; set; }
        public string Amount { get; set; }
    }
}