using Opdex.Platform.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.Vaults
{
    /// <summary>
    /// A request to quote setting the vault owner.
    /// </summary>
    public class SetVaultOwnerQuoteRequest
    {
        /// <summary>Address of the new owner.</summary>
        [Required]
        public Address Owner { get; set; }
    }
}
