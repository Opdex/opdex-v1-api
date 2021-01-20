using System;
using Opdex.Core.Common.Extensions;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Indexer.Domain.Models.LogEvents;

namespace Opdex.Indexer.Domain.Models
{
    public class TransactionLog
    {
        public TransactionLog(TransactionLogDto transactionLogDto)
        {
            if (!transactionLogDto.Address.HasValue())
            {
                throw new ArgumentNullException(nameof(transactionLogDto.Address));
            }
            
            if (transactionLogDto.Log == null)
            {
                throw new ArgumentNullException(nameof(transactionLogDto.Log));
            }

            Address = transactionLogDto.Address;
            Log = transactionLogDto.Log;
            DeserializeLog();
        }
        
        public string Address { get; private set; }
        
        public object Log { get; private set; }
        
        public ILogEvent Event { get; private set; }

        private void DeserializeLog()
        {
            var eventType = Log.GetType().GetProperty("EventType")?.GetValue(Log, null).ToString();

            if (!eventType.HasValue()) return;

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