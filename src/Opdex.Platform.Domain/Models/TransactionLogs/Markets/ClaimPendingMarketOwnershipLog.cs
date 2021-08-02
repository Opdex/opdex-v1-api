namespace Opdex.Platform.Domain.Models.TransactionLogs.Markets
{
    public class ClaimPendingMarketOwnershipLog : OwnershipLog
    {
        public ClaimPendingMarketOwnershipLog(dynamic log, string address, int sortOrder)
            : base(TransactionLogType.ClaimPendingMarketOwnershipLog, (string)log?.from, (string)log?.to, address, sortOrder)
        {
        }

        public ClaimPendingMarketOwnershipLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(TransactionLogType.ClaimPendingMarketOwnershipLog, id, transactionId, address, sortOrder, details)
        {
        }
    }
}
