using System;
using MediatR;
using Opdex.Core.Domain.Models.TransactionEvents;
using Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionEvents;

namespace Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionEvents
{
    public class PersistTransactionEventSummaryCommand : IRequest<bool>
    {
        public PersistTransactionEventSummaryCommand(TransactionEvent txEvent, long eventId)
        {
            Enum.TryParse(typeof(TransactionEventType), txEvent.EventType, out var eventType);
            EventTypeId = (int)eventType; 
            TransactionId = txEvent.TransactionId;
            EventId = eventId;
            Contract = txEvent.Address;
            SortOrder = txEvent.SortOrder;
        }
        
        public long TransactionId { get; }
        public int EventTypeId { get; }
        public long EventId { get; }
        public string Contract { get; }
        public long SortOrder { get; }
    }
}