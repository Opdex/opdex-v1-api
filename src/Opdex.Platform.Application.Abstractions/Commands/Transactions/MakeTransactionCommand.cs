using System;
using MediatR;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.Transactions;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions
{
    public class MakeTransactionCommand : IRequest<ulong>
    {
        public MakeTransactionCommand(Transaction transaction)
        {
            Transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        }

        public Transaction Transaction { get; }
    }
}
