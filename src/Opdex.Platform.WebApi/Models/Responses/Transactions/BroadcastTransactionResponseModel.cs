using Opdex.Platform.Common.Models;
using System.Diagnostics.CodeAnalysis;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions
{
    public class BroadcastTransactionResponseModel
    {
        [NotNull]
        public Sha256 TxHash { get; set; }

        [NotNull]
        public Address Sender { get; set; }
    }
}
