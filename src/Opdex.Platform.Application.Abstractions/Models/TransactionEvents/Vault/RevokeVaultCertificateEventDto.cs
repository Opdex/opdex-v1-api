namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Vault
{
    public class RevokeVaultCertificateEventDto : TransactionEventDto
    {
        public string Holder { get; set; }
        public string NewAmount { get; set; }
        public string OldAmount { get; set; }
        public ulong VestedBlock { get; set; }
    }
}
