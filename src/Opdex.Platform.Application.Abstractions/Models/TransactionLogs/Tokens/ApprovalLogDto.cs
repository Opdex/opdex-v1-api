namespace Opdex.Platform.Application.Abstractions.Models.TransactionLogs.Tokens
{
    public class ApprovalLogDto : TransactionLogDto
    {
        public string Owner { get; set; }
        public string Spender { get; set; }
        public string Amount { get; set; }
    }
}