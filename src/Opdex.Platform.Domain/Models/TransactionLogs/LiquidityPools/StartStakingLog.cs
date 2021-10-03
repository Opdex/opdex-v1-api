using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools
{
    public class StartStakingLog : StakeLog
    {
        public StartStakingLog(dynamic log, Address address, int sortOrder)
            : base(TransactionLogType.StartStakingLog, (string)log?.staker, UInt256.Parse((string)log?.amount),
                   UInt256.Parse((string)log?.totalStaked), UInt256.Parse((string)log?.stakerBalance), address, sortOrder)
        {
        }

        public StartStakingLog(ulong id, ulong transactionId, Address address, int sortOrder, string details)
            : base(TransactionLogType.StartStakingLog, id, transactionId, address, sortOrder, details)
        {
        }
    }
}
