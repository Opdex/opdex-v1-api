using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions
{
    /// <summary>Attempts to notify a user of a transaction which has been broadcast.</summary>
    public class CreateNotifyUserOfTransactionBroadcastCommand : IRequest<bool>
    {
        public CreateNotifyUserOfTransactionBroadcastCommand(Address user, string transactionHash)
        {
            User = user != Address.Empty ? user : throw new ArgumentNullException(nameof(user), "User address must be set.");
            TransactionHash = transactionHash.HasValue() ? transactionHash : throw new ArgumentNullException(nameof(transactionHash), "Transaction hash must be set.");
        }

        public Address User { get; }
        public string TransactionHash { get; }
    }
}
