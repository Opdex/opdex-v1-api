using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Requests.Vaults
{
    /// <summary>
    /// A request to quote the creation of a vault certificate.
    /// </summary>
    public class CreateVaultCertificateQuoteRequest
    {
        /// <summary>Address of the certificate holder.</summary>
        public Address Holder { get; set; }

        /// <summary>Amount of staking tokens to assign.</summary>
        public FixedDecimal Amount { get; set; }
    }
}
