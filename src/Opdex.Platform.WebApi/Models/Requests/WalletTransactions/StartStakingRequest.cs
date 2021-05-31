namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class StartStakingRequest : LocalWalletCredentials
    {
        /// <summary>
        /// The amount of tokens to stake.
        /// </summary>
        public string Amount { get; set; }
        
        /// <summary>
        /// The address of the liquidity pool to stake in.
        /// </summary>
        public string LiquidityPool { get; set; }
    }
}