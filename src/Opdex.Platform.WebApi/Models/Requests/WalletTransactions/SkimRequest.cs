namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class SkimRequest : LocalWalletCredentials
    {
        /// <summary>
        /// The recipient of the skimmed tokens.
        /// </summary>
        public string Recipient { get; set; }
        
        /// <summary>
        /// The liquidity pool being skimmed.
        /// </summary>
        public string LiquidityPool { get; set; }
    }
}