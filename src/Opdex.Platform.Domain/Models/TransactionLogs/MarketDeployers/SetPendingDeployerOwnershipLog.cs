namespace Opdex.Platform.Domain.Models.TransactionLogs.MarketDeployers
{
    public class SetPendingDeployerOwnershipLog : OwnershipLog
    {
        public SetPendingDeployerOwnershipLog(dynamic log, string address, int sortOrder)
            : base(TransactionLogType.SetPendingDeployerOwnershipLog, (string)log?.from, (string)log?.to, address, sortOrder)
        {
        }

        public SetPendingDeployerOwnershipLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(TransactionLogType.SetPendingDeployerOwnershipLog, id, transactionId, address, sortOrder, details)
        {
        }
    }
}
