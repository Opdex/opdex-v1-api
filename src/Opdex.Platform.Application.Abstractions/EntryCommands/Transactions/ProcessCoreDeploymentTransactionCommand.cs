using MediatR;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;

public class ProcessCoreDeploymentTransactionCommand : IRequest<Unit>
{
    public ProcessCoreDeploymentTransactionCommand(Sha256 txHash)
    {
        TxHash = txHash;
    }

    public Sha256 TxHash { get; }
}