using Opdex.Platform.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.Vaults
{
    /// <summary>
    /// A request to quote the creation of a vault certificate.
    /// </summary>
    public class CreateVaultCertificateQuoteRequest
    {
        /// <summary>Address of the certificate holder.</summary>
        [Required]
        public Address Holder { get; set; }

        /// <summary>Amount of staking tokens to assign.</summary>
        [Required]
        public FixedDecimal Amount { get; set; }
    }
}
