using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Opdex.Platform.Domain.Models.Transactions
{
    public class Transaction
    {
        public Transaction(string txHash, ulong blockHeight, int gasUsed, string from, string to, bool success,
                           string newContractAddress, IList<TransactionLog> logs)
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

            if (!from.HasValue())
            {
                throw new ArgumentNullException(nameof(from), "From address must be set.");
            }

            if (!to.HasValue() && !newContractAddress.HasValue())
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

        public Transaction(long id, string txHash, ulong blockHeight, int gasUsed, string from, string to, bool success,
                           string newContractAddress)
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

        public long Id { get; private set; }
        public string Hash { get; }
        public ulong BlockHeight { get; }
        public int GasUsed { get; }
        public string From { get; }
        public string To { get; }
        public bool Success { get; }
        public string NewContractAddress { get; }

        public IList<TransactionLog> Logs { get; }

        // Todo: maybe enum - unknown | maybe | yeah | no
        public bool IsOpdexTx { get; private set; }

        public IList<T> LogsOfType<T>(TransactionLogType logType) where T : TransactionLog
        {
            return Logs.Where(log => log.LogType == logType).Select(log => log as T).ToList();
        }

        public IEnumerable<TransactionLog> LogsOfTypes(IEnumerable<TransactionLogType> logTypes)
        {
            return Logs.Where(log => logTypes.Contains(log.LogType)).ToList();
        }

        public IDictionary<string, List<TransactionLog>> GroupedLogsOfTypes(IEnumerable<TransactionLogType> logTypes)
        {
            return LogsOfTypes(logTypes)
                .GroupBy(log => log.Contract)
                .ToDictionary(k => k.FirstOrDefault()?.Contract, logs => logs.ToList());
        }

        public void SetId(long id)
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
