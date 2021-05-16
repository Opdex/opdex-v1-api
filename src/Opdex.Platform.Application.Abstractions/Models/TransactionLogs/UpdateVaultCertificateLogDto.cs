namespace Opdex.Platform.Application.Abstractions.Models.TransactionLogs
{
    public class UpdateVaultCertificateLogDto : TransactionLogDto
    {
        public string Owner { get; set; }
        public string Amount { get; set; }
        public ulong VestedBlock { get; set; }
    }
}