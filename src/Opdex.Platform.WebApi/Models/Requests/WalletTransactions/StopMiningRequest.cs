using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class StopMiningRequest
    {
        /// <summary>
        /// The liquidity pool contract address to exit mining tokens for.
        /// </summary>
        public string LiquidityPool { get; set; }

        /// <summary>
        /// The amount of tokens to stop mining with.
        /// </summary>
        [Required]
        public string Amount { get; set; }
    }
}
