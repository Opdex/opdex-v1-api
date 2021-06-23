namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class ApproveAllowanceRequest
    {
        /// <summary>
        /// The address of the token's smart contract.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// The allowance amount to approve
        /// </summary>
        public string Amount { get; set; }

        /// <summary>
        /// The spender of the allowance.
        /// </summary>
        public string Spender { get; set; }
    }
}
