using System;
using MediatR;
using Opdex.Core.Domain.Models.TransactionLogs;
using Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionLogs;

namespace Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionLogs
{
    public class PersistTransactionLogSummaryCommand : IRequest<bool>
    {
        public PersistTransactionLogSummaryCommand(TransactionLog txLog, long logId)
        {
            Enum.TryParse(typeof(TransactionLogType), txLog.LogType, out var logType);
            LogTypeId = (int)logType; 
            TransactionId = txLog.TransactionId;
            LogId = logId;
            Contract = txLog.Address;
            SortOrder = txLog.SortOrder;
        }
        
        public long TransactionId { get; }
        public int LogTypeId { get; }
        public long LogId { get; }
        public string Contract { get; }
        public long SortOrder { get; }
    }
}