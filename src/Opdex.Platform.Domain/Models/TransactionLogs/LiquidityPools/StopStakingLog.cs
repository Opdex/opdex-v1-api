using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;

public class StopStakingLog : StakeLog
{
    public StopStakingLog(dynamic log, Address address, int sortOrder)
        : base(TransactionLogType.StopStakingLog, (string)log?.staker, UInt256.Parse((string)log?.amount),
               UInt256.Parse((string)log?.totalStaked), UInt256.Parse((string)log?.stakerBalance), address, sortOrder)
    {
    }

    public StopStakingLog(ulong id, ulong transactionId, Address address, int sortOrder, string details)
        : base(TransactionLogType.StopStakingLog, id, transactionId, address, sortOrder, details)
    {
    }
}