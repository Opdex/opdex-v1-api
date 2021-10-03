using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Domain.Models.TransactionLogs.Vaults
{
    public class ClaimPendingVaultOwnershipLog : OwnershipLog
    {
        public ClaimPendingVaultOwnershipLog(dynamic log, Address address, int sortOrder)
            : base(TransactionLogType.ClaimPendingVaultOwnershipLog, (string)log?.from, (string)log?.to, address, sortOrder)
        {
        }

        public ClaimPendingVaultOwnershipLog(ulong id, ulong transactionId, Address address, int sortOrder, string details)
            : base(TransactionLogType.ClaimPendingVaultOwnershipLog, id, transactionId, address, sortOrder, details)
        {
        }
    }
}
