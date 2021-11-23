using Opdex.Platform.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Responses.Vaults
{
    /// <summary>
    /// Vault certificate details.
    /// </summary>
    public class VaultCertificateResponseModel
    {
        /// <summary>
        /// Address of the certificate owner.
        /// </summary>
        /// <example>tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm</example>
        public Address Owner { get; set; }

        /// <summary>
        /// Total value of the certificate, over the duration of the vesting period.
        /// </summary>
        /// <example>250000.00000000</example>
        public FixedDecimal Amount { get; set; }

        /// <summary>
        /// Block that the vesting period starts.
        /// </summary>
        /// <example>500000</example>
        [Range(1, double.MaxValue)]
        public ulong VestingStartBlock { get; set; }

        /// <summary>
        /// Block that the vesting period ends.
        /// </summary>
        /// <example>750000</example>
        [Range(1, double.MaxValue)]
        public ulong VestingEndBlock { get; set; }

        /// <summary>
        /// Whether the certificate has been redeemed by the certificate owner.
        /// </summary>
        /// <example>false</example>
        public bool Redeemed { get; set; }

        /// <summary>
        /// Whether the certificate was revoked by the vault owner.
        /// </summary>
        /// <example>false</example>
        public bool Revoked { get; set; }
    }
}
