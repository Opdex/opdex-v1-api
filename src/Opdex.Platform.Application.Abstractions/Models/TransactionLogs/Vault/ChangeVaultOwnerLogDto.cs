namespace Opdex.Platform.Application.Abstractions.Models.TransactionLogs.Vault
{
    public class ChangeVaultOwnerLogDto : TransactionLogDto
    {
        public string From { get; set; }
        public string To { get; set; }
    }
}