using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Domain.Models.TransactionLogs.MiningPools
{
    public class StartMiningLog : MineLog
    {
        public StartMiningLog(dynamic log, Address address, int sortOrder)
            : base(TransactionLogType.StartMiningLog, (string)log?.miner, UInt256.Parse((string)log?.amount),
                   UInt256.Parse((string)log?.totalSupply), UInt256.Parse((string)log?.minerBalance), address, sortOrder)
        {
        }

        public StartMiningLog(ulong id, ulong transactionId, Address address, int sortOrder, string details)
            : base(TransactionLogType.StartMiningLog, id, transactionId, address, sortOrder, details)
        {
        }
    }
}
