using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Requests.Vaults
{
    /// <summary>
    /// A request to quote the creation of a vault certificate.
    /// </summary>
    public class CreateVaultCertificateQuoteRequest
    {
        /// <summary>Address of the certificate holder.</summary>
        /// <example>tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm</example>
        public Address Holder { get; set; }

        /// <summary>Amount of staking tokens to assign.</summary>
        /// <example>50000.00000000</example>
        public FixedDecimal Amount { get; set; }
    }
}
