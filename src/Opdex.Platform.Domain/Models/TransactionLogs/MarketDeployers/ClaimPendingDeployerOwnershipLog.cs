using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Domain.Models.TransactionLogs.MarketDeployers
{
    public class ClaimPendingDeployerOwnershipLog : OwnershipLog
    {
        public ClaimPendingDeployerOwnershipLog(dynamic log, Address address, int sortOrder)
            : base(TransactionLogType.ClaimPendingDeployerOwnershipLog, (string)log?.from, (string)log?.to, address, sortOrder)
        {
        }

        public ClaimPendingDeployerOwnershipLog(ulong id, ulong transactionId, Address address, int sortOrder, string details)
            : base(TransactionLogType.ClaimPendingDeployerOwnershipLog, id, transactionId, address, sortOrder, details)
        {
        }
    }
}
