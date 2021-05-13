namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.ODX
{
    public class VaultCertificateEntity
    {
        public long Id { get; set; }
        public long VaultId { get; set; }
        public string Owner { get; set; }
        public string Amount { get; set; }
        public ulong VestedBlock { get; set; }
        public bool Redeemed { get; set; }
    }
}