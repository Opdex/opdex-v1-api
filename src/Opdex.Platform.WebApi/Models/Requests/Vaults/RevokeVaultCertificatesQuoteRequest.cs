using Opdex.Platform.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.Vaults
{
    /// <summary>
    /// A request to quote revoking the vault certificates for a certificate holder.
    /// </summary>
    public class RevokeVaultCertificatesQuoteRequest
    {
        /// <summary>Address of the certificate holder.</summary>
        [Required]
        public Address Holder { get; set; }
    }
}
