using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class RewardMiningPoolsRequest
    {
        /// <summary>
        /// Flag determining if all or only one mining pool should be rewarded tokens for mining.
        /// </summary>
        [Required]
        public bool FullDistribution { get; set; }
    }
}
