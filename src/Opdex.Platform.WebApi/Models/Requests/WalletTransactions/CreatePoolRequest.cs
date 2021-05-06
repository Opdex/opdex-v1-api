namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class CreatePoolRequest : LocalWalletCredentials
    {
        /// <summary>
        /// The SRC token's smart contract address.
        /// </summary>
        public string Token { get; set; }
        
        /// <summary>
        /// The address of the market to add the pool to.
        /// </summary>
        public string Market { get; set; }
    }
}