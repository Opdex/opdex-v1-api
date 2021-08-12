using System;
using MediatR;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.Transactions;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Transactions
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