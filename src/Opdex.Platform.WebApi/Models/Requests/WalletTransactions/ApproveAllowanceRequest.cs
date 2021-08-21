using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class ApproveAllowanceRequest
    {
        /// <summary>
        /// The address of the token's smart contract.
        /// </summary>
        [Required]
        public string Token { get; set; }

        /// <summary>
        /// The allowance amount to approve
        /// </summary>
        [Required]
        public string Amount { get; set; }

        /// <summary>
        /// The spender of the allowance.
        /// </summary>
        [Required]
        public string Spender { get; set; }
    }
}
