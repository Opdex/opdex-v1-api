using MediatR;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.Transactions
{
    public class RetrieveTransactionsWithFilterQuery : IRequest<IEnumerable<Transaction>>
    {
        public RetrieveTransactionsWithFilterQuery(TransactionsCursor cursor)
        {
            Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Cursor must be set.");
        }

        public TransactionsCursor Cursor { get; }
    }
}
