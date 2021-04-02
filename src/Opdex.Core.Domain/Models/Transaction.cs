using System;
using System.Collections.Generic;
using System.Linq;
using Opdex.Core.Common.Extensions;
using Opdex.Core.Domain.Models.TransactionEvents;

namespace Opdex.Core.Domain.Models
{
    public class Transaction
    {
        public Transaction(string txHash, ulong blockHeight, int gasUsed, string from, string to)
        {
            if (!txHash.HasValue())
            {
                throw new ArgumentNullException(nameof(txHash));
            }
            
            if (blockHeight == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight));
            }
            
            if (gasUsed == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(gasUsed));
            }
            
            if (!from.HasValue())
            {
                throw new ArgumentNullException(nameof(from));
            }
            
            if (!to.HasValue())
            {
                throw new ArgumentNullException(nameof(to));
            }

            Hash = txHash;
            BlockHeight = blockHeight;
            GasUsed = gasUsed;
            From = from;
            To = to;
            Events = new List<TransactionEvent>();
        }

        public Transaction(long id, string txHash, ulong blockHeight, int gasUsed, string from, string to, IEnumerable<TransactionEvent> events)
        {
            Id = id;
            Hash = txHash;
            BlockHeight = blockHeight;
            GasUsed = gasUsed;
            From = from;
            To = to;
            Events = new List<TransactionEvent>();
            AttachEvents(events);
        }
        
        public Transaction(long id, string txHash, ulong blockHeight, int gasUsed, string from, string to)
        {
            Id = id;
            Hash = txHash;
            BlockHeight = blockHeight;
            GasUsed = gasUsed;
            From = from;
            To = to;
            Events = new List<TransactionEvent>();
        }
        
        public long Id { get; }
        public string Hash { get; }
        public ulong BlockHeight { get; }
        public int GasUsed { get; }
        public string From { get; }
        public string To { get; }
        public ICollection<TransactionEvent> Events { get; }
        public IReadOnlyCollection<string> PoolsEngaged { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="poolsEngaged"></param>
        /// <exception cref="Exception"></exception>
        public void SetPoolsEngaged(List<string> poolsEngaged)
        {
            if (!poolsEngaged.All(p => Events.Any(e => e.Address == p)))
            {
                throw new Exception("Pool engagement not found in transaction events");
            }

            PoolsEngaged = poolsEngaged.Select(p => p).ToList();
        }

        private void AttachEvents(IEnumerable<TransactionEvent> events)
        {
            foreach (var txEvent in events)
            {
                if (txEvent.TransactionId == 0)
                {
                    txEvent.SetTransactionId(Id);
                }

                Events.Add(txEvent);
            }
        }
        
        public void AttachEvent(TransactionEvent transactionEvent)
        {
            if (Id == 0 || transactionEvent.Id == 0)
            {
                throw new Exception($"Unable to add transaction event on incomplete {nameof(Transaction)}.");
            }

            // Todo: Insert into sorted order
            Events.Add(transactionEvent);
        }

        public void DeserializeEvent(string address, string topic, int sortOrder, dynamic log)
        {
            if (!topic.Contains("Opdex")) return;

            topic = topic.Replace("Opdex", "");

            try
            {
                TransactionEvent opdexEvent = topic switch
                {
                    nameof(SyncEvent) => new SyncEvent(log, address, sortOrder),
                    nameof(BurnEvent) => new BurnEvent(log, address, sortOrder),
                    nameof(MintEvent) => new MintEvent(log, address, sortOrder),
                    nameof(SwapEvent) => new SwapEvent(log, address, sortOrder),
                    nameof(ApprovalEvent) => new ApprovalEvent(log, address, sortOrder),
                    nameof(TransferEvent) => new TransferEvent(log, address, sortOrder),
                    "TransferLog" => new TransferEvent(log, address, sortOrder),
                    nameof(PoolCreatedEvent) => new PoolCreatedEvent(log, address, sortOrder),
                    _ => null
                };

                if (opdexEvent == null) return;

                Events.Add(opdexEvent);
            }
            catch
            {
                // ignored
                // Maybe we want to keep this around incase other events in this transaction
                // are Opdex events
            }
        }
    }
}