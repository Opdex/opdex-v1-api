using Opdex.Platform.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class TransactionBroadcastNotificationRequest
    {
        [Required]
        public Address WalletAddress { get; set; }

        [Required]
        public string TransactionHash { get; set; }
    }
}
