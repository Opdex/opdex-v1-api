using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class DistributeTokensRequest
    {
        /// <summary>
        /// The token address to distribute tokens for.
        /// </summary>
        public Address Token { get; set; }
    }
}
