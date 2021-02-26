using System;
using MediatR;
using Opdex.Core.Domain.Models.Transaction;

namespace Opdex.Indexer.Infrastructure.Abstractions.Data.Commands
{
    public class PersistTransactionCommand : IRequest<long>
    {
        public PersistTransactionCommand(Transaction transaction)
        {
            Transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        }
        
        public Transaction Transaction { get; }
    }
}