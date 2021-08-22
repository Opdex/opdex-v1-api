using System;

namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class CollectMiningRewardsRequest
    {
        /// <summary>
        /// The liquidity pool contract address to collect mined tokens for.
        /// </summary>
        [Obsolete] // Delete property when removing WalletBroadcast endpoints and flows.
        public string LiquidityPool { get; set; }
    }
}
