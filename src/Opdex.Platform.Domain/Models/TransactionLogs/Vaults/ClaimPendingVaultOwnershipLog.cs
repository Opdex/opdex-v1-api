namespace Opdex.Platform.Domain.Models.TransactionLogs.Vaults
{
    public class ClaimPendingVaultOwnershipLog : OwnershipLog
    {
        public ClaimPendingVaultOwnershipLog(dynamic log, string address, int sortOrder)
            : base(TransactionLogType.ClaimPendingVaultOwnershipLog, (string)log?.from, (string)log?.to, address, sortOrder)
        {
        }

        public ClaimPendingVaultOwnershipLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(TransactionLogType.ClaimPendingVaultOwnershipLog, id, transactionId, address, sortOrder, details)
        {
        }
    }
}
