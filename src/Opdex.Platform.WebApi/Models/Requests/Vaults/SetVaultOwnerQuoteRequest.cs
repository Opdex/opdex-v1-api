using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Requests.Vaults
{
    /// <summary>
    /// A request to quote setting the vault owner.
    /// </summary>
    public class SetVaultOwnerQuoteRequest
    {
        /// <summary>Address of the new owner.</summary>
        public Address Owner { get; set; }
    }
}
