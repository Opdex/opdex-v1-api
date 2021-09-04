using Opdex.Platform.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.Markets
{
    /// <summary>
    /// Request for a quote to collect fees from a standard market.
    /// </summary>
    public class CollectMarketFeesQuoteRequest
    {
        [Required]
        public Address Token { get; set; }

        [Required]
        public FixedDecimal Amount { get; set; }
    }
}
