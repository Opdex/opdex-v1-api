using System;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class StartStakingRequest
    {
        /// <summary>
        /// The amount of tokens to stake.
        /// </summary>
        [Required]
        public string Amount { get; set; }

        /// <summary>
        /// The address of the liquidity pool to stake in.
        /// </summary>
        [Obsolete] // Delete property when removing WalletBroadcast endpoints and flows.
        public string LiquidityPool { get; set; }
    }
}
