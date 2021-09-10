using Opdex.Platform.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class StartMiningRequest
    {
        /// <summary>
        /// The amount of liquidity pool tokens to use for mining.
        /// </summary>
        [Required]
        public FixedDecimal Amount { get; set; }

        /// <summary>
        /// The liquidity pool contract address to start mining tokens for.
        /// </summary>
        ///
        public Address LiquidityPool { get; set; }
    }
}
