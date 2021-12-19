using MediatR;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;

/// <summary>Attempts to notify a user of a transaction which has been broadcast.</summary>
public class CreateNotifyUserOfTransactionBroadcastCommand : IRequest<bool>
{
    public CreateNotifyUserOfTransactionBroadcastCommand(Sha256 transactionHash)
    {
        TransactionHash = transactionHash;
    }

    public Sha256 TransactionHash { get; }
}
