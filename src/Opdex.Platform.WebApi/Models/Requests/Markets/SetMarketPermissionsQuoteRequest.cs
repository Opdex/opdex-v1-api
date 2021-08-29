using Opdex.Platform.Domain.Models.Markets;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.Markets
{
    /// <summary>
    /// Request to set permissions for a specific address in a standard market.
    /// </summary>
    public class SetMarketPermissionsQuoteRequest
    {
        [Required]
        public Permissions Permission { get; set; }

        [Required]
        public bool Authorize { get; set; }
    }
}
