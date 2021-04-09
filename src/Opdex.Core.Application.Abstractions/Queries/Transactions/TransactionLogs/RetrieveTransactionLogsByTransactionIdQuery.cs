using System;
using System.Collections.Generic;
using MediatR;
using Opdex.Core.Domain.Models.TransactionLogs;

namespace Opdex.Core.Application.Abstractions.Queries.Transactions.TransactionLogs
{
    public class RetrieveTransactionLogsByTransactionIdQuery : IRequest<IEnumerable<TransactionLog>>
    {
        public RetrieveTransactionLogsByTransactionIdQuery(long transactionId)
        {
            if (transactionId < 1)
            {
                throw new ArgumentNullException(nameof(transactionId));
            }
            
            TransactionId = transactionId;
        }
        
        public long TransactionId { get; }
    }
}