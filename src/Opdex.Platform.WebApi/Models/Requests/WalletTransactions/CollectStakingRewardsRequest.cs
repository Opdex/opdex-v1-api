namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class CollectStakingRewardsRequest
    {
        /// <summary>
        /// An option to liquidate earned liquidity pool tokens from staking back into
        /// the pool's reserves tokens.
        /// </summary>
        public bool Liquidate { get; set; }

        /// <summary>
        /// The address of the liquidity pool to collect from.
        /// </summary>
        public string LiquidityPool { get; set; }
    }
}
