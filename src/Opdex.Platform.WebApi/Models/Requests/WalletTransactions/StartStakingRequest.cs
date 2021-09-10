using Opdex.Platform.Common.Models;
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
        public FixedDecimal Amount { get; set; }

        /// <summary>
        /// The address of the liquidity pool to stake in.
        /// </summary>
        [Obsolete] // Delete property when removing WalletBroadcast endpoints and flows.
        public Address LiquidityPool { get; set; }
    }
}
