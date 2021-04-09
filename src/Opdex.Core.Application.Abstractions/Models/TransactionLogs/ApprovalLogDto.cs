namespace Opdex.Core.Application.Abstractions.Models.TransactionLogs
{
    public class ApprovalLogDto : TransactionLogDto
    {
        public string Owner { get; set; }
        public string Spender { get; set; }
        public string Amount { get; set; }
    }
}