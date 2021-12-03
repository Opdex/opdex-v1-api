using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Domain.Models.TransactionLogs.Markets;

public class ClaimPendingMarketOwnershipLog : OwnershipLog
{
    public ClaimPendingMarketOwnershipLog(dynamic log, Address address, int sortOrder)
        : base(TransactionLogType.ClaimPendingMarketOwnershipLog, (string)log?.from, (string)log?.to, address, sortOrder)
    {
    }

    public ClaimPendingMarketOwnershipLog(ulong id, ulong transactionId, Address address, int sortOrder, string details)
        : base(TransactionLogType.ClaimPendingMarketOwnershipLog, id, transactionId, address, sortOrder, details)
    {
    }
}