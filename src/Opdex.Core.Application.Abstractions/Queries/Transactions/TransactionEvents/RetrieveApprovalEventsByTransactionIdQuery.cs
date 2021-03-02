using System;
using System.Collections.Generic;
using MediatR;
using Opdex.Core.Domain.Models.TransactionEvents;

namespace Opdex.Core.Application.Abstractions.Queries.Transactions.TransactionEvents
{
    public class RetrieveApprovalEventsByTransactionIdQuery : IRequest<IEnumerable<ApprovalEvent>>
    {
        public RetrieveApprovalEventsByTransactionIdQuery(long transactionId)
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