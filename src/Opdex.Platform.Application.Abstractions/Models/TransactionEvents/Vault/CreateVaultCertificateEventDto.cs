using Opdex.Platform.Application.Abstractions.Models.TransactionLogs;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Vault
{
    public class CreateVaultCertificateEventDto : TransactionEventDto
    {
        public string Holder { get; set; }
        public string Amount { get; set; }
        public ulong VestedBlock { get; set; }
    }
}
