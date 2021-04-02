using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;
using Opdex.Core.Domain.Models.TransactionEvents;

namespace Opdex.Core.Application.Abstractions.Queries.Transactions.TransactionEvents
{
    public class RetrieveApprovalEventsByIdsQuery : IRequest<IEnumerable<ApprovalEvent>>
    {
        public RetrieveApprovalEventsByIdsQuery(IEnumerable<TransactionEventSummary> txEvents)
        {
            var eventIds = txEvents as TransactionEventSummary[] ?? txEvents.ToArray();
            
            if (!eventIds.Any() || eventIds.Any(t => t.EventId < 1))
            {
                throw new ArgumentOutOfRangeException(nameof(txEvents));
            }

            TransactionEvents = eventIds;
        }
        
        public IEnumerable<TransactionEventSummary> TransactionEvents { get; }
    }
}