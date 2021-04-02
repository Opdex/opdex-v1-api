using System;
using MediatR;
using Opdex.Core.Common.Extensions;

namespace Opdex.Indexer.Application.Abstractions.EntryCommands.Transactions
{
    public class CreateTransactionCommand : IRequest<bool>
    {
        public CreateTransactionCommand(string txHash)
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