using System;
using System.Collections.Generic;
using MediatR;
using Opdex.Core.Domain.Models.TransactionLogs;

namespace Opdex.Core.Infrastructure.Abstractions.Data.Queries.Transactions.TransactionLogs
{
    public class SelectTransactionLogsByTransactionIdQuery : IRequest<IEnumerable<TransactionLog>>
    {
        public SelectTransactionLogsByTransactionIdQuery(long transactionId)
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