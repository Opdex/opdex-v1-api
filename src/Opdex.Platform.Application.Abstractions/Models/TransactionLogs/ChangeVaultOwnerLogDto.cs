namespace Opdex.Platform.Application.Abstractions.Models.TransactionLogs
{
    public class ChangeVaultOwnerLogDto : TransactionLogDto
    {
        public string From { get; set; }
        public string To { get; set; }
    }
}