using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Requests.Transactions
{
    /// <summary>
    /// Request to notify a user that their transaction has been broadcast.
    /// </summary>
    public class TransactionBroadcastNotificationRequest
    {
        public Address WalletAddress { get; set; }

        public Sha256 TransactionHash { get; set; }
    }
}
