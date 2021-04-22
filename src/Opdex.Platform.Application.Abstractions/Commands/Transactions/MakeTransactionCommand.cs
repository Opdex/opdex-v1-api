using System;
using MediatR;
using Opdex.Platform.Domain.Models;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions
{
    public class MakeTransactionCommand : IRequest<bool>
    {
        public MakeTransactionCommand(Transaction transaction)
        {
            Transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        }
        
        public Transaction Transaction { get; }
    }
}