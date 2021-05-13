namespace Opdex.Platform.Application.Abstractions.Models.TransactionLogs
{
    public class VaultCertificateCreatedLogDto : TransactionLogDto
    {
        public string Owner { get; set; }
        public string Amount { get; set; }
        public ulong VestedBlock { get; set; }
    }
}