namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class StopStakingRequest : LocalWalletCredentials
    {
        /// <summary>
        /// An option to liquidate earned liquidity pool tokens from staking back into
        /// the pool's reserves tokens.
        /// </summary>
        public bool Liquidate { get; set; }
        
        /// <summary>
        /// The address of the liquidity pool to stop staking in.
        /// </summary>
        public string LiquidityPool { get; set; }
        
        /// <summary>
        /// The amount of tokens to stop staking.
        /// </summary>
        public string Amount { get; set; }
    }
}