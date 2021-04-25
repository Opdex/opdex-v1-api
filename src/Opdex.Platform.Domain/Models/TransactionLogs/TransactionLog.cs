using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models.TransactionLogs
{
    public abstract class TransactionLog
    {
        protected internal TransactionLog(TransactionLogType logType, string contract, int sortOrder)
        {
            if (logType == TransactionLogType.Unknown)
            {
                throw new ArgumentOutOfRangeException(nameof(logType));
            }
            
            if (!contract.HasValue())
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

        protected internal TransactionLog(TransactionLogType logType, long id, long transactionId, string contract, int sortOrder)
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
        public string Contract { get; }
        public int SortOrder { get; }

        protected internal void SetTransactionId(long txId)
        {
            if (TransactionId == 0 && txId > 0)
            {
                TransactionId = txId;
            }
        }

        public abstract string SerializeLogDetails();
    }
}