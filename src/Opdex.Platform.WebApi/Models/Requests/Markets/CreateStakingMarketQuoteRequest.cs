using Opdex.Platform.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.Markets
{
    /// <summary>
    /// Request to quote the creation of a staking market.
    /// </summary>
    public class CreateStakingMarketQuoteRequest
    {
        [Required]
        public Address StakingToken { get; set; }
    }
}
