using MediatR;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Transactions
{
    public class GetTransactionsWithFilterQuery : IRequest<TransactionsDto>
    {
        public GetTransactionsWithFilterQuery(TransactionsCursor cursor)
        {
            Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Cursor must be set.");
        }

        public TransactionsCursor Cursor { get; }
    }
}
