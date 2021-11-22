using Opdex.Platform.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Responses.Vaults
{
    public class VaultCertificateResponseModel
    {
        public Address Owner { get; set; }

        public FixedDecimal Amount { get; set; }

        [Range(1, double.MaxValue)]
        public ulong VestingStartBlock { get; set; }

        [Range(1, double.MaxValue)]
        public ulong VestingEndBlock { get; set; }

        public bool Redeemed { get; set; }
        
        public bool Revoked { get; set; }
    }
}
