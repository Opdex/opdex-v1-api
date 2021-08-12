using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.Transactions;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions
{
    public class SelectTransactionsWithFilterQuery : IRequest<IEnumerable<Transaction>>
    {
        public SelectTransactionsWithFilterQuery(TransactionsCursor cursor)
        {
            Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Cursor must be set.");
        }

        public TransactionsCursor Cursor { get; }
    }
}
