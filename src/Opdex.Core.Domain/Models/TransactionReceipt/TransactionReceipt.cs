using System;
using System.Collections.Generic;
using System.Linq;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models.TransactionReceipt
{
    public class TransactionReceipt
    {
        public TransactionReceipt(string txHash, ulong blockHeight, int gasUsed, string from, string to, bool success, dynamic[] logs)
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

            if (!success)
            {
                throw new ArgumentException($"Transaction must be a {nameof(success)}", nameof(success));
            }
            
            if (!logs.Any())
            {
                throw new ArgumentException("Transaction Receipt must include logs", nameof(logs));
            }

            Hash = txHash;
            BlockHeight = blockHeight;
            GasUsed = gasUsed;
            From = from;
            To = to;
            DeserializeEvents(logs);
        }
        
        public string Hash { get; private set; }
        public ulong BlockHeight { get; private set; }
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

        /// <summary>
        /// Deserializes all logs as events in the transaction
        /// </summary>
        /// <param name="logs">dynamic list of logs to deserialize</param>
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