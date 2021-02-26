using System;
using MediatR;
using Opdex.Core.Domain.Models;

namespace Opdex.Indexer.Application.Abstractions.Commands
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