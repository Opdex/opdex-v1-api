using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    /// <summary>
    /// A request to quote rewarding mining pools with governance tokens.
    /// </summary>
    public class RewardMiningPoolsRequest
    {
        /// <summary>
        /// Flag determining if all or only one mining pool should be rewarded tokens for mining.
        /// </summary>
        /// <example>true</example>
        [Required]
        public bool FullDistribution { get; set; }
    }
}
