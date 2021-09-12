using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Domain.Events
{
    public class TransactionBroadcastNotification : INotification
    {
        public TransactionBroadcastNotification(Address user, string txHash)
        {
            User = user != Address.Empty ? user : throw new ArgumentNullException(nameof(user), "User address must be set.");
            TxHash = txHash.HasValue() ? txHash : throw new ArgumentNullException(nameof(txHash), "Transaction hash must be set.");
        }

        public Address User { get; }
        public string TxHash { get; }
    }
}
