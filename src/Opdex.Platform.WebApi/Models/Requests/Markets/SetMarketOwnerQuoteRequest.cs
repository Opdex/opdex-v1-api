using Opdex.Platform.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.Markets
{
    /// <summary>
    /// Requests for a quote to set a new market owner.
    /// </summary>
    public class SetMarketOwnerQuoteRequest
    {
        /// <summary>Address of the new owner.</summary>
        [Required]
        public Address Owner { get; set; }
    }
}
