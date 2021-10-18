using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.TransactionLogs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Opdex.Platform.Domain.Models.Transactions
{
    public class Transaction
    {
        public Transaction(string txHash, ulong blockHeight, int gasUsed, Address from, Address to, bool success,
                           Address newContractAddress, IList<TransactionLog> logs)
        {
            if (!txHash.HasValue())
            {
                throw new ArgumentNullException(nameof(txHash), "Transaction hash must be set.");
            }

            if (blockHeight == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than 0.");
            }

            if (gasUsed == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(gasUsed), "Transaction gas must be set.");
            }

            if (from == Address.Empty)
            {
                throw new ArgumentNullException(nameof(from), "From address must be set.");
            }

            if (to == Address.Empty && newContractAddress == Address.Empty)
            {
                throw new ArgumentNullException(nameof(to), "To address must be set.");
            }

            Hash = txHash;
            BlockHeight = blockHeight;
            GasUsed = gasUsed;
            From = from;
            To = to;
            Success = success;
            NewContractAddress = newContractAddress;
            Logs = logs ?? new List<TransactionLog>();
        }

        public Transaction(ulong id, string txHash, ulong blockHeight, int gasUsed, Address from, Address to, bool success, Address newContractAddress)
        {
            Id = id;
            Hash = txHash;
            BlockHeight = blockHeight;
            GasUsed = gasUsed;
            From = from;
            To = to;
            Success = success;
            NewContractAddress = newContractAddress;
            Logs = new List<TransactionLog>();
        }

        public ulong Id { get; private set; }
        public string Hash { get; }
        public ulong BlockHeight { get; }
        public int GasUsed { get; }
        public Address From { get; }
        public Address To { get; }
        public bool Success { get; }
        public Address NewContractAddress { get; }

        public IList<TransactionLog> Logs { get; }

        public IList<T> LogsOfType<T>(TransactionLogType logType) where T : TransactionLog
        {
            return Logs.Where(log => log.LogType == logType).Select(log => log as T).ToList();
        }

        public IEnumerable<TransactionLog> LogsOfTypes(IEnumerable<TransactionLogType> logTypes)
        {
            return Logs.Where(log => logTypes.Contains(log.LogType)).ToList();
        }

        public IDictionary<Address, List<TransactionLog>> GroupedLogsOfTypes(IEnumerable<TransactionLogType> logTypes)
        {
            return LogsOfTypes(logTypes)
                .GroupBy(log => log.Contract)
                .ToDictionary(k => k.First().Contract, logs => logs.ToList());
        }

        public void SetLog(TransactionLog log)
        {
            if (log.Id == 0)
            {
                throw new Exception("Invalid transaction log.");
            }

            if (Logs.Any(l => l.Id == log.Id))
            {
                return;
            }

            Logs.Add(log);
        }

        public void SetId(ulong id)
        {
            if (Id != 0)
            {
                throw new InvalidOperationException("TransactionId already set.");
            }

            Id = id;

            foreach (var log in Logs)
            {
                log.SetTransactionId(Id);
            }
        }
    }
}
