namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class DistributeTokensRequest : LocalWalletCredentials
    {
        /// <summary>
        /// The token address to distribute tokens for.
        /// </summary>
        public string Token { get; set; }
    }
}