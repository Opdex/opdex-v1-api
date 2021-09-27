using MediatR;
using Opdex.Platform.Domain.Models.Transactions;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions
{
    public class SelectTransactionsForSnapshotRewindQuery : IRequest<IEnumerable<Transaction>>
    {
        public SelectTransactionsForSnapshotRewindQuery(DateTime dateTime)
        {
            if (dateTime == default)
            {
                throw new ArgumentOutOfRangeException(nameof(dateTime), "Date time must be valid.");
            }

            DateTime = dateTime;
        }

        public DateTime DateTime { get; }
    }
}
