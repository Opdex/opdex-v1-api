namespace Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionLogs
{
    public class ApprovalLogEntity : LogEntityBase
    {
        public string Owner { get; set; }
        public string Spender { get; set; }
        public string Amount { get; set; }
    }
}