namespace Opdex.Platform.Application.Abstractions.Models.Vaults
{
    public class VaultCertificateDto
    {
        public string Owner { get; set; }
        public string Amount { get; set; }
        public ulong VestingStartBlock { get; set; }
        public ulong VestingEndBlock { get; set; }
        public bool Redeemed { get; set; }
        public bool Revoked { get; set; }
    }
}
