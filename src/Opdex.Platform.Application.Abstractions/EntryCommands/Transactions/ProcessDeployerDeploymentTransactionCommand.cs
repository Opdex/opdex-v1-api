using System;
using MediatR;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions
{
    public class ProcessDeployerDeploymentTransactionCommand : IRequest<Unit>
    {
        public ProcessDeployerDeploymentTransactionCommand(string txHash)
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