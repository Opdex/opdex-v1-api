using MediatR;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;

/// <summary>Attempts to notify a user of a transaction which has been broadcast.</summary>
public class CreateNotifyUserOfTransactionBroadcastCommand : IRequest<bool>
{
    public CreateNotifyUserOfTransactionBroadcastCommand(Sha256 transactionHash, Address sender)
    {
        TransactionHash = transactionHash;
        Sender = sender;
    }

    public Sha256 TransactionHash { get; }
    public Address Sender { get; }
}
