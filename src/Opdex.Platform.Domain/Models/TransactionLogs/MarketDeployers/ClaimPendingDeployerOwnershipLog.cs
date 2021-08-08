namespace Opdex.Platform.Domain.Models.TransactionLogs.MarketDeployers
{
    public class ClaimPendingDeployerOwnershipLog : OwnershipLog
    {
        public ClaimPendingDeployerOwnershipLog(dynamic log, string address, int sortOrder)
            : base(TransactionLogType.ClaimPendingDeployerOwnershipLog, (string)log?.from, (string)log?.to, address, sortOrder)
        {
        }

        public ClaimPendingDeployerOwnershipLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(TransactionLogType.ClaimPendingDeployerOwnershipLog, id, transactionId, address, sortOrder, details)
        {
        }
    }
}
