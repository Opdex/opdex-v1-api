using System;
using System.Collections.Generic;
using MediatR;
using Opdex.Core.Domain.Models.TransactionLogs;

namespace Opdex.Core.Application.Abstractions.Queries.Transactions.TransactionLogs
{
    public class RetrieveTransactionLogSummariesByTransactionIdQuery : IRequest<List<TransactionLogSummary>>
    {
        public RetrieveTransactionLogSummariesByTransactionIdQuery(long transactionId)
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