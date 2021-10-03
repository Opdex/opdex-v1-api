using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Domain.Models.TransactionLogs.MarketDeployers
{
    public class SetPendingDeployerOwnershipLog : OwnershipLog
    {
        public SetPendingDeployerOwnershipLog(dynamic log, Address address, int sortOrder)
            : base(TransactionLogType.SetPendingDeployerOwnershipLog, (string)log?.from, (string)log?.to, address, sortOrder)
        {
        }

        public SetPendingDeployerOwnershipLog(ulong id, ulong transactionId, Address address, int sortOrder, string details)
            : base(TransactionLogType.SetPendingDeployerOwnershipLog, id, transactionId, address, sortOrder, details)
        {
        }
    }
}
