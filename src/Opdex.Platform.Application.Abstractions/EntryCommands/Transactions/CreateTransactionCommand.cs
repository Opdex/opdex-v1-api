using MediatR;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;

public class CreateTransactionCommand : IRequest<bool>
{
    public CreateTransactionCommand(Sha256 txHash)
    {
        TxHash = txHash;
    }

    public Sha256 TxHash { get; }
}