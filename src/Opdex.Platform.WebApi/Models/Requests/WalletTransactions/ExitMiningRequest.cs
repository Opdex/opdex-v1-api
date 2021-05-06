namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class ExitMiningRequest : LocalWalletCredentials
    {
        /// <summary>
        /// The liquidity pool contract address to exit mining tokens for.
        /// </summary>
        public string LiquidityPool { get; set; }
    }
}