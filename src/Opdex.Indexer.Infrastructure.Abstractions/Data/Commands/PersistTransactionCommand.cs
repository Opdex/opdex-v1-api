using System;
using System.Transactions;
using MediatR;

namespace Opdex.Indexer.Infrastructure.Abstractions.Data.Commands
{
    public class PersistTransactionCommand : IRequest<bool>
    {
        public PersistTransactionCommand(Transaction transaction)
        {
            Transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        }
        
        public Transaction Transaction { get; }
    }
}