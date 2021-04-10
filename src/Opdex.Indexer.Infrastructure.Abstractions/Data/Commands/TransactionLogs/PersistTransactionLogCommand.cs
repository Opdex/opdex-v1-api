using System;
using MediatR;
using Newtonsoft.Json;
using Opdex.Core.Domain.Models.TransactionLogs;
using Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionLogs;

namespace Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionLogs
{
    public class PersistTransactionLogCommand  : IRequest<bool>
    {
        public PersistTransactionLogCommand(TransactionLog txLog)
        {
            Enum.TryParse(typeof(TransactionLogType), txLog.LogType, out var logType);
            LogTypeId = (int)logType;
            TransactionId = txLog.TransactionId;
            Contract = txLog.Contract;
            SortOrder = txLog.SortOrder;
            Details = txLog.SerializeLogDetails();
        }
        
        public long TransactionId { get; }
        public int LogTypeId { get; }
        public string Contract { get; }
        public long SortOrder { get; }
        public string Details { get; }
    }
}