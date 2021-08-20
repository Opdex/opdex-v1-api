using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class SkimRequest
    {
        /// <summary>
        /// The recipient of the skimmed tokens.
        /// </summary>
        public Address Recipient { get; set; }

        /// <summary>
        /// The liquidity pool being skimmed.
        /// </summary>
        public string LiquidityPool { get; set; }
    }
}
