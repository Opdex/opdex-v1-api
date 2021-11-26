using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Vault
{
    /// <summary>
    /// Revoke vault certificate event.
    /// </summary>
    public class RevokeVaultCertificateEvent : TransactionEvent
    {
        /// <summary>
        /// Address of the certificate holder.
        /// </summary>
        /// <example>tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm</example>
        public Address Holder { get; set; }

        /// <summary>
        /// Final certificate value.
        /// </summary>
        /// <example>"25000.00000000"</example>
        public FixedDecimal NewAmount { get; set; }

        /// <summary>
        /// Original certificate value.
        /// </summary>
        /// <example>"50000.00000000"</example>
        public FixedDecimal OldAmount { get; set; }

        /// <summary>
        /// End block of the vesting period.
        /// </summary>
        /// <example>750000</example>
        public ulong VestedBlock { get; set; }
    }
}
