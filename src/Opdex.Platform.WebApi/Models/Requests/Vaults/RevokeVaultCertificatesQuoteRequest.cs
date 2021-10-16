using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Requests.Vaults
{
    /// <summary>
    /// A request to quote revoking the vault certificates for a certificate holder.
    /// </summary>
    public class RevokeVaultCertificatesQuoteRequest
    {
        /// <summary>Address of the certificate holder.</summary>
        public Address Holder { get; set; }
    }
}
