namespace Opdex.Platform.Domain.Models.TransactionLogs.Markets
{
    public class SetPendingMarketOwnershipLog : OwnershipLog
    {
        public SetPendingMarketOwnershipLog(dynamic log, string address, int sortOrder)
            : base(TransactionLogType.SetPendingMarketOwnershipLog, (string)log?.from, (string)log?.to, address, sortOrder)
        {
        }

        public SetPendingMarketOwnershipLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(TransactionLogType.SetPendingMarketOwnershipLog, id, transactionId, address, sortOrder, details)
        {
        }
    }
}