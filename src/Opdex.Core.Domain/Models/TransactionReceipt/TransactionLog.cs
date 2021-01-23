using System;
using Opdex.Core.Common.Extensions;
using Opdex.Indexer.Domain.Models;
using Opdex.Indexer.Domain.Models.LogEvents;

namespace Opdex.Core.Domain.Models.TransactionReceipt
{
    public class TransactionLog
    {
        public TransactionLog(dynamic transactionEventLog)
        {
            string address = transactionEventLog?.Address;
            object log = transactionEventLog?.log;
            
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
            DeserializeLog();
        }
        
        /// <summary>
        /// The contract address that logged this event
        /// </summary>
        public string Address { get; private set; }
        
        /// <summary>
        /// The original log object - to be removed soon
        /// </summary>
        public object Log { get; private set; }
        
        /// <summary>
        /// Deserialized log event
        /// </summary>
        public ILogEvent Event { get; private set; }

        private void DeserializeLog()
        {
            var eventType = Log.GetType().GetProperty("EventType")?.GetValue(Log, null).ToString();

            if (!eventType.HasValue()) return;

            // Todo: These should be created via constructor with validations
            Event = eventType switch
            {
                nameof(SyncEvent) => Log as SyncEvent,
                nameof(BurnEvent) => Log as BurnEvent,
                nameof(MintEvent) => Log as MintEvent,
                nameof(SwapEvent) => Log as SwapEvent,
                nameof(ApprovalEvent) => Log as ApprovalEvent,
                nameof(TransferEvent) => Log as TransferEvent,
                nameof(PairCreatedEvent) => Log as PairCreatedEvent,
                _ => null
            };
        }
    }
}