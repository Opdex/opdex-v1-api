using Opdex.Platform.Common.Models;
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
        public FixedDecimal Amount { get; set; }

        /// <summary>
        /// The spender of the allowance.
        /// </summary>
        [Required]
        public Address Spender { get; set; }
    }
}
