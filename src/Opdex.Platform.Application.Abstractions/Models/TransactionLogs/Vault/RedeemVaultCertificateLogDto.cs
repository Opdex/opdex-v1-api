namespace Opdex.Platform.Application.Abstractions.Models.TransactionLogs.Vault
{
    public class RedeemVaultCertificateLogDto : TransactionLogDto
    {
        public string Owner { get; set; }
        public string Amount { get; set; }
        public ulong VestedBlock { get; set; }
    }
}