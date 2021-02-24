using System;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models.TransactionReceipt.LogEvents
{
    public class LogEventBase : ILogEvent
    {
        protected LogEventBase(string eventType)
        {
            if (!eventType.HasValue())
            {
                throw new ArgumentNullException(nameof(eventType));
            }
            
            EventType = eventType;
        }

        internal LogEventBase(string eventType, long id, long transactionId, string address)
        {
            EventType = eventType;
            Id = id;
            TransactionId = transactionId;
            Address = address;
        }
        
        public string EventType { get; }
        public long Id { get; private set; }
        public long TransactionId { get; }
        public string Address { get; private set; }

        public void SetAddress(string address)
        {
            if (Address.HasValue() || !address.HasValue())
            {
                throw new ArgumentException($"{nameof(Address)} is already set.", nameof(Address));
            }

            Address = address;
        }

        public void SetTransactionId(long id)
        {
            if (Id == 0 && id > 0)
            {
                Id = id;
            }
        }
    }
}