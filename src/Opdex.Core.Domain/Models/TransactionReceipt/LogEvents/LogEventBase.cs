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

        internal LogEventBase(string evenType, long id, long transactionId)
        {
            EventType = evenType;
            Id = id;
            TransactionId = transactionId;
        }
        
        public string EventType { get; set; }
        public long Id { get; }
        public long TransactionId { get; }
    }
}