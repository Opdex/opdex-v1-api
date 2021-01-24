using System;
using Opdex.Core.Common.Extensions;
using Opdex.Core.Domain.Models.TransactionReceipt.LogEvents;

namespace Opdex.Core.Domain.Models.TransactionReceipt
{
    public class TransactionLog
    {
        public TransactionLog(dynamic transactionEventLog)
        {
            string address = transactionEventLog?.Address;
            var log = transactionEventLog?.Log;
            
            if (!address.HasValue())
            {
                throw new ArgumentNullException(nameof(address));
            }
            
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }

            Address = address;
            Log = log;
            
            // Todo: maybe this should be called externally when we need types of logs
            // Exceptions can be thrown during creation of domain models but maybe those
            // should be just thrown away since we're pulling from Cirrus and any contract
            // transaction can return the event type we're trying to deserialize.
            //
            // Maybe some type of service that only deserializes events that are from known 
            // pair contracts.
            DeserializeLog();
        }
        
        /// <summary>
        /// The contract address that logged this event
        /// </summary>
        public string Address { get; private set; }
        
        /// <summary>
        /// The original log object - to be removed soon
        /// </summary>
        public dynamic Log { get; private set; }
        
        /// <summary>
        /// Deserialized log event
        /// </summary>
        public ILogEvent Event { get; private set; }

        private void DeserializeLog()
        {
            string eventType = Log?.eventType;

            if (!eventType.HasValue()) return;

            Event = eventType switch
            {
                nameof(SyncEvent) => new SyncEvent(Log),
                nameof(BurnEvent) => new BurnEvent(Log),
                nameof(MintEvent) => new MintEvent(Log),
                nameof(SwapEvent) => new SwapEvent(Log),
                nameof(ApprovalEvent) => new ApprovalEvent(Log),
                nameof(TransferEvent) => new TransferEvent(Log),
                nameof(PairCreatedEvent) => new PairCreatedEvent(Log),
                _ => null
            };
        }
    }
}