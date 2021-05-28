namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class StartMiningRequest : LocalWalletCredentials
    {
        /// <summary>
        /// The amount of liquidity pool tokens to use for mining.
        /// </summary>
        public string Amount { get; set; }
        
        /// <summary>
        /// The liquidity pool contract address to start mining tokens for.
        /// </summary>
        public string LiquidityPool { get; set; }
    }
}