using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Domain.Models.TransactionLogs.Markets;

public class SetPendingMarketOwnershipLog : OwnershipLog
{
    public SetPendingMarketOwnershipLog(dynamic log, Address address, int sortOrder)
        : base(TransactionLogType.SetPendingMarketOwnershipLog, (string)log?.from, (string)log?.to, address, sortOrder)
    {
    }

    public SetPendingMarketOwnershipLog(ulong id, ulong transactionId, Address address, int sortOrder, string details)
        : base(TransactionLogType.SetPendingMarketOwnershipLog, id, transactionId, address, sortOrder, details)
    {
    }
}