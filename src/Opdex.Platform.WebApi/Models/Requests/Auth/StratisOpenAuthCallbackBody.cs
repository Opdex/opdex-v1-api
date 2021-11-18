using Opdex.Platform.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.Auth
{
    /// <summary>
    /// Callback body for Stratis Open Auth Protocol.
    /// </summary>
    public class StratisOpenAuthCallbackBody
    {
        /// <summary>
        /// Signed Stratis ID callback.
        /// </summary>
        /// <example></example>
        [Required]
        public string Signature { get; set; }

        /// <summary>
        /// Message signer wallet address.
        /// </summary>
        /// <example>tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm</example>
        [Required]
        public Address PublicKey { get; set; }
    }
}