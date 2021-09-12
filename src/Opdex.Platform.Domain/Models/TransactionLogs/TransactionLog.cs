using System;
using Newtonsoft.Json;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Domain.Models.TransactionLogs
{
    public abstract class TransactionLog
    {
        protected internal TransactionLog(TransactionLogType logType, Address contract, int sortOrder)
        {
            if (!logType.IsValid())
            {
                throw new ArgumentOutOfRangeException(nameof(logType));
            }

            if (contract == Address.Empty)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            if (sortOrder < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sortOrder));
            }

            LogType = logType;
            Contract = contract;
            SortOrder = sortOrder;
        }

        protected internal TransactionLog(TransactionLogType logType, long id, long transactionId, Address contract, int sortOrder)
        {
            LogType = logType;
            Id = id;
            TransactionId = transactionId;
            Contract = contract;
            SortOrder = sortOrder;
        }

        public long Id { get; }
        public TransactionLogType LogType { get; }
        public long TransactionId { get; private set; }
        public Address Contract { get; }
        public int SortOrder { get; }

        protected internal void SetTransactionId(long txId)
        {
            // Todo: Maybe throw
            if (TransactionId == 0 && txId > 0)
            {
                TransactionId = txId;
            }
        }

        public abstract string SerializeLogDetails();
    }
}
