using System;
using MediatR;
using Opdex.Indexer.Domain.Models;

namespace Opdex.Indexer.Application.Abstractions.Commands
{
    public class MakeTransactionCommand : IRequest<bool>
    {
        public MakeTransactionCommand(TransactionReceipt transaction)
        {
            Transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        }
        
        public TransactionReceipt Transaction { get; }
    }
}