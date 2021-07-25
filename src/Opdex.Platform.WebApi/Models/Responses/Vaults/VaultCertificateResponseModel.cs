namespace Opdex.Platform.WebApi.Models.Responses.Vaults
{
    public class VaultCertificateResponseModel
    {
        public string Owner { get; set; }
        public string Amount { get; set; }
        public ulong VestingStartBlock { get; set; }
        public ulong VestingEndBlock { get; set; }
        public bool Redeemed { get; set; }
        public bool Revoked { get; set; }
    }
}
