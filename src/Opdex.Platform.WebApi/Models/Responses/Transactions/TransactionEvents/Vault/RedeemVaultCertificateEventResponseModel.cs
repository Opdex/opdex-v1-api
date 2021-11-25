using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Vault
{
    /// <summary>
    /// Redeem vault certificate event.
    /// </summary>
    public class RedeemVaultCertificateEventResponseModel : TransactionEventResponseModel
    {
        /// <summary>
        /// Address of the certificate holder.
        /// </summary>
        /// <example>tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm</example>
        public Address Holder { get; set; }

        /// <summary>
        /// Value of the certificate.
        /// </summary>
        /// <example>"25000.00000000"</example>
        public FixedDecimal Amount { get; set; }

        /// <summary>
        /// End block of the vesting period.
        /// </summary>
        /// <example>750000</example>
        public ulong VestedBlock { get; set; }
    }
}
