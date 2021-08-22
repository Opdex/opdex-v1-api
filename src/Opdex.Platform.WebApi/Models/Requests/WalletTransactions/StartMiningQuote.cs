using Opdex.Platform.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class MiningQuote
    {
        /// <summary>
        /// The amount of liquidity pool tokens to use for the quote.
        /// </summary>
        [Required]
        public string Amount { get; set; }
    }
}
