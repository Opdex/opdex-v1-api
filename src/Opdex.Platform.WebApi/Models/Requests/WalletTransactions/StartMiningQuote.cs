using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class MiningQuote
    {
        /// <summary>
        /// The amount of liquidity pool tokens to use for the quote.
        /// </summary>
        public string Amount { get; set; }
    }
}
