using MediatR;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;

public class CreateTransactionCommand : IRequest<bool>
{
    public CreateTransactionCommand(Sha256 txHash, bool notify)
    {
        TxHash = txHash;
        Notify = notify;
    }

    public Sha256 TxHash { get; }
    public bool Notify { get; }
}
