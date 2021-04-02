using System;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models.TransactionEvents
{
    public abstract class TransactionEvent
    {
        protected internal TransactionEvent(string eventType, string address, int sortOrder)
        {
            if (!eventType.HasValue())
            {
                throw new ArgumentNullException(nameof(eventType));
            }
            
            if (!address.HasValue())
            {
                throw new ArgumentNullException(nameof(address));
            }
            
            if (sortOrder < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sortOrder));
            }
            
            EventType = eventType;
            Address = address;
            SortOrder = sortOrder;
        }

        protected internal TransactionEvent(string eventType, long id, long transactionId, string address, int sortOrder)
        {
            EventType = eventType;
            Id = id;
            TransactionId = transactionId;
            Address = address;
            SortOrder = sortOrder;
        }
        
        public long Id { get; }
        public string EventType { get; }
        public long TransactionId { get; private set; }
        public string Address { get; }
        public int SortOrder { get; }

        protected internal void SetTransactionId(long txId)
        {
            if (TransactionId == 0 && txId > 0)
            {
                TransactionId = txId;
            }
        }
    }
}