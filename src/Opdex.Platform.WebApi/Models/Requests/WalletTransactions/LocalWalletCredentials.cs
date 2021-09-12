using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class LocalWalletCredentials
    {
        /// <summary>
        /// The wallet address of the transaction sender.
        /// </summary>
        public Address WalletAddress { get; set; }

        /// <summary>
        /// The name of the local wallet.
        /// </summary>
        public string WalletName { get; set; }

        /// <summary>
        /// The password to the local wallet.
        /// </summary>
        public string WalletPassword { get; set; }
    }
}
