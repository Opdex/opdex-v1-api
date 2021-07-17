namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Vault
{
    public class RedeemVaultCertificateEventDto : TransactionEventDto
    {
        public string Holder { get; set; }
        public string Amount { get; set; }
        public ulong VestedBlock { get; set; }
    }
}
