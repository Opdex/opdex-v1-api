namespace Opdex.Core.Application.Abstractions.Models.TransactionLogs
{
    public class TransferLogDto : TransactionLogDto
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Amount { get; set; }
    }
}