namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class StartMiningQuote
    {
        /// <summary>
        /// The amount of liquidity pool tokens to use for mining.
        /// </summary>
        public string Amount { get; set; }

        /// <summary>
        /// The mining pool contract address to start mining tokens in.
        /// </summary>
        public string MiningPool { get; set; }
    }
}
