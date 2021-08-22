using System;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    [Obsolete] // Delete this request when the braodcast wallet flow is removed.
    public class CreatePoolRequest
    {
        /// <summary>
        /// The SRC token's smart contract address.
        /// </summary>
        [Required]
        public string Token { get; set; }
    }
}
