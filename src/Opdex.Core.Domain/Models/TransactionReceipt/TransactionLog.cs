using System;
using System.Linq;
using Opdex.Core.Common.Extensions;
using Opdex.Core.Domain.Models.TransactionReceipt.LogEvents;

namespace Opdex.Core.Domain.Models.TransactionReceipt
{
    public class TransactionLog
    {
        public TransactionLog(dynamic transactionEventLog)
        {
            string address = transactionEventLog?.Address ?? string.Empty;
            string[] topics = transactionEventLog?.Topics ?? new string[0];
            var log = transactionEventLog?.Log;
            
            if (!address.HasValue())
            {
                throw new ArgumentNullException(nameof(address));
            }

            if (topics.Any() != true)
            {
                throw new ArgumentOutOfRangeException(nameof(topics));
            }
            
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }

            Topics = topics;
            Address = address;
            DeserializeLog(log);
        }
        
        /// <summary>
        /// The contract address that logged this event
        /// </summary>
        public string Address { get; private set; }
        
        /// <summary>
        /// Transaction topics including the logged type and indexed properties.
        /// </summary>
        /// <remarks>
        /// - Topics[0] will be the deserialized log event name (e.g. OpdexSyncEvent)
        /// - The remaining Topics are serialized, indexed property values
        /// </remarks>
        public string[] Topics { get; private set; }
        
        /// <summary>
        /// Deserialized log event
        /// </summary>
        public ILogEvent Event { get; private set; }
        
        /// <summary>
        /// The sort order of the event in the transaction
        /// </summary>
        public int SortOrder { get; private set; }
        
        /// <summary>
        /// Deserializes the log, sets the event depending on type
        /// </summary>
        /// <param name="log"></param>
        private void DeserializeLog(dynamic log)
        {
            var eventType = Topics[0];

            Event = eventType switch
            {
                nameof(SyncEvent) => new SyncEvent(log),
                nameof(BurnEvent) => new BurnEvent(log),
                nameof(MintEvent) => new MintEvent(log),
                nameof(SwapEvent) => new SwapEvent(log),
                nameof(ApprovalEvent) => new ApprovalEvent(log),
                nameof(TransferEvent) => new TransferEvent(log),
                nameof(PairCreatedEvent) => new PairCreatedEvent(log),
                _ => null
            };

            Event?.SetAddress(Address);
        }

        public void UpdateSortOrder(int index)
        {
            SortOrder = index;
        }
    }
}