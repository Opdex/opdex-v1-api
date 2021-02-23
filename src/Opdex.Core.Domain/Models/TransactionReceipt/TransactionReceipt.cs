using System;
using System.Collections.Generic;
using System.Linq;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models.TransactionReceipt
{
    public class TransactionReceipt
    {
        public TransactionReceipt(string txHash, string blockHash, int gasUsed, string from, string to, bool success, dynamic[] logs)
        {
            if (!txHash.HasValue())
            {
                throw new ArgumentNullException(nameof(txHash));
            }
            
            if (!blockHash.HasValue())
            {
                throw new ArgumentNullException(nameof(blockHash));
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

            if (!success)
            {
                throw new ArgumentException($"Transaction must be a {nameof(success)}", nameof(success));
            }
            
            if (!logs.Any())
            {
                throw new ArgumentException("Transaction Receipt must include logs", nameof(logs));
            }

            Hash = txHash;
            BlockHash = blockHash;
            GasUsed = gasUsed;
            From = from;
            To = to;
            DeserializeEvents(logs);
        }
        
        public string Hash { get; private set; }
        public string BlockHash { get; private set; }
        public int GasUsed { get; private set; }
        public string From { get; private set; }
        public string To { get; private set; }
        public IReadOnlyCollection<TransactionLog> Events { get; private set; }
        public IReadOnlyCollection<string> PairsEngaged { get; private set; }

        public void SetPairsEngaged(List<string> pairsEngaged)
        {
            if (!pairsEngaged.All(p => Events.Any(e => e.Address == p)))
            {
                throw new Exception("Pair engagement not found in transaction events");
            }

            PairsEngaged = pairsEngaged.Select(p => p).ToList();
        }

        private void DeserializeEvents(dynamic[] logs)
        {
            var events = new List<TransactionLog>();
            
            foreach (var log in logs)
            {
                try
                {
                    var transactionLog = new TransactionLog(log);

                    if (transactionLog.Event != null)
                    {
                        events.Add(transactionLog);
                    }
                }
                catch (Exception)
                {
                    // Intentionally throw unmatched events away
                }
            }

            Events = events;
        }
    }
}