using Opdex.Platform.Common.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class CollectStakingRewardsRequest
    {
        /// <summary>
        /// An option to liquidate earned liquidity pool tokens from staking back into
        /// the pool's reserves tokens.
        /// </summary>
        [Required]
        public bool Liquidate { get; set; }

        /// <summary>
        /// The address of the liquidity pool to collect from.
        /// </summary>
        [Obsolete] // Delete property when removing WalletBroadcast endpoints and flows.
        public Address LiquidityPool { get; set; }
    }
}
