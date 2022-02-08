using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Domain.Models.TransactionLogs.Tokens;

public class OwnershipTransferredLog : OwnershipLog
{
    public OwnershipTransferredLog(dynamic log, Address contract, int sortOrder)
        : base(TransactionLogType.OwnershipTransferredLog, (string)log?.previousOwner, (string)log?.newOwner,
            contract, sortOrder)
    {
    }

    public OwnershipTransferredLog(ulong id, ulong transactionId, Address address, int sortOrder, string details)
        : base(TransactionLogType.OwnershipTransferredLog, id, transactionId, address, sortOrder, details)
    {
    }
}
