using System;
using System.Collections.Generic;
using MediatR;
using Opdex.Core.Domain.Models.TransactionLogs;

namespace Opdex.Core.Infrastructure.Abstractions.Data.Queries.Transactions.TransactionLogs
{
    public class SelectTransactionLogSummariesByTransactionIdQuery : IRequest<IEnumerable<TransactionLogSummary>>
    {
        public SelectTransactionLogSummariesByTransactionIdQuery(long transactionId)
        {
            if (transactionId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(transactionId));
            }

            TransactionId = transactionId;
        }
        
        public long TransactionId { get; }
    }
}