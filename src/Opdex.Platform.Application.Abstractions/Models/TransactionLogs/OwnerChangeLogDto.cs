namespace Opdex.Platform.Application.Abstractions.Models.TransactionLogs
{
    public class OwnerChangeLogDto : TransactionLogDto
    {
        public string From { get; set; }
        public string To { get; set; }
    }
}