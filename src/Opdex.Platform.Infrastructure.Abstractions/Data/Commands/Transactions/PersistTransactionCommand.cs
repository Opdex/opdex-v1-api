using System;
using MediatR;
using Opdex.Platform.Domain.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands
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