using MediatR;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions;

/// <summary>Attempts to notify a user of a transaction which has been broadcast.</summary>
public class MakeNotifyUserOfTransactionBroadcastCommand : IRequest
{
    public MakeNotifyUserOfTransactionBroadcastCommand(Address user, Sha256 transactionHash)
    {
        User = user != Address.Empty ? user : throw new ArgumentNullException(nameof(user), "User address must be set.");
        TransactionHash = transactionHash;
    }

    public Address User { get; }
    public Sha256 TransactionHash { get; }
}
