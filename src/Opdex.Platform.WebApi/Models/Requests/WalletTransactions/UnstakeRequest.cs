namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class UnstakeRequest : LocalWalletCredentials
    {
        /// <summary>
        /// The recipient collecting staking rewards.
        /// </summary>
        public string Recipient { get; set; }
        
        /// <summary>
        /// An option to liquidate earned liquidity pool tokens from staking back into
        /// the pool's reserves tokens.
        /// </summary>
        public bool Liquidate { get; set; }
        
        /// <summary>
        /// The address of the liquidity pool to stop staking in.
        /// </summary>
        public string LiquidityPool { get; set; }
    }
}