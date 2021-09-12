using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Vaults
{
    public class VaultCertificateResponseModel
    {
        public Address Owner { get; set; }
        public FixedDecimal Amount { get; set; }
        public ulong VestingStartBlock { get; set; }
        public ulong VestingEndBlock { get; set; }
        public bool Redeemed { get; set; }
        public bool Revoked { get; set; }
    }
}
