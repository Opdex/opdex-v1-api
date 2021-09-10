using Opdex.Platform.Common.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class StopMiningRequest
    {
        /// <summary>
        /// The liquidity pool contract address to exit mining tokens for.
        /// </summary>
        [Obsolete] // Delete property when removing WalletBroadcast endpoints and flows.
        public Address LiquidityPool { get; set; }

        /// <summary>
        /// The amount of tokens to stop mining with.
        /// </summary>
        [Required]
        public FixedDecimal Amount { get; set; }
    }
}
