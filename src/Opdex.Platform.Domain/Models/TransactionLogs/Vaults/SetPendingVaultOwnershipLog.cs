using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Domain.Models.TransactionLogs.Vaults
{
    public class SetPendingVaultOwnershipLog : OwnershipLog
    {
        public SetPendingVaultOwnershipLog(dynamic log, Address address, int sortOrder)
            : base(TransactionLogType.SetPendingVaultOwnershipLog, (string)log?.from, (string)log?.to, address, sortOrder)
        {
        }

        public SetPendingVaultOwnershipLog(long id, long transactionId, Address address, int sortOrder, string details)
            : base(TransactionLogType.SetPendingVaultOwnershipLog, id, transactionId, address, sortOrder, details)
        {
        }
    }
}
