using System;
using MediatR;
using Opdex.Core.Domain.Models;

namespace Opdex.Indexer.Infrastructure.Abstractions.Data.Commands
{
    public class PersistTransactionCommand : IRequest<Transaction>
    {
        public PersistTransactionCommand(Transaction transaction)
        {
            Transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        }
        
        public Transaction Transaction { get; }
    }
}