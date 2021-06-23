namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class CollectMiningRewardsRequest
    {
        /// <summary>
        /// The liquidity pool contract address to collect mined tokens for.
        /// </summary>
        public string LiquidityPool { get; set; }
    }
}
