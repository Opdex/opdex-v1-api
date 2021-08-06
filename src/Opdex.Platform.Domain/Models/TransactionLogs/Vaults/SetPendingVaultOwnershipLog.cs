namespace Opdex.Platform.Domain.Models.TransactionLogs.Vaults
{
    public class SetPendingVaultOwnershipLog : OwnershipLog
    {
        public SetPendingVaultOwnershipLog(dynamic log, string address, int sortOrder)
            : base(TransactionLogType.SetPendingVaultOwnershipLog, (string)log?.from, (string)log?.to, address, sortOrder)
        {
        }

        public SetPendingVaultOwnershipLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(TransactionLogType.SetPendingVaultOwnershipLog, id, transactionId, address, sortOrder, details)
        {
        }
    }
}
