using System;
using MediatR;
using Opdex.Core.Domain.Models.TransactionReceipt;

namespace Opdex.Indexer.Infrastructure.Abstractions.Data.Commands
{
    public class PersistTransactionCommand : IRequest<long>
    {
        public PersistTransactionCommand(TransactionReceipt transaction)
        {
            Transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        }
        
        public TransactionReceipt Transaction { get; }
    }
}