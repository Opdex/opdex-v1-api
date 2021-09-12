using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Transactions.TransactionLogs
{
    public class PersistTransactionLogCommand : IRequest<bool>
    {
        public PersistTransactionLogCommand(TransactionLog txLog)
        {
            LogTypeId = (int)txLog.LogType;
            TransactionId = txLog.TransactionId;
            Contract = txLog.Contract;
            SortOrder = txLog.SortOrder;
            Details = txLog.SerializeLogDetails();
        }

        public long TransactionId { get; }
        public int LogTypeId { get; }
        public Address Contract { get; }
        public long SortOrder { get; }
        public string Details { get; }
    }
}
