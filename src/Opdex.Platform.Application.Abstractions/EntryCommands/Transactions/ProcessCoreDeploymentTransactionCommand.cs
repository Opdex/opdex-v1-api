using System;
using MediatR;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions
{
    public class ProcessCoreDeploymentTransactionCommand : IRequest<Unit>
    {
        public ProcessCoreDeploymentTransactionCommand(string txHash)
        {
            if (!txHash.HasValue())
            {
                throw new ArgumentNullException(nameof(txHash));
            }

            TxHash = txHash;
        }

        public string TxHash { get; }
    }
}
