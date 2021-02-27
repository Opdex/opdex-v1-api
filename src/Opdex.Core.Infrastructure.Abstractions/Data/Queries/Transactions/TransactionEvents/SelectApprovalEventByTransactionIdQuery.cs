using System;
using MediatR;
using Opdex.Core.Domain.Models.TransactionEvents;

namespace Opdex.Core.Infrastructure.Abstractions.Data.Queries.Transactions.TransactionEvents
{
    public class SelectApprovalEventByTransactionIdQuery : IRequest<ApprovalEvent>
    {
        public SelectApprovalEventByTransactionIdQuery(long transactionId)
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