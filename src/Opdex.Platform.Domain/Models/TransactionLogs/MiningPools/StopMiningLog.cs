using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Domain.Models.TransactionLogs.MiningPools;

public class StopMiningLog : MineLog
{
    public StopMiningLog(dynamic log, Address address, int sortOrder)
        : base(TransactionLogType.StopMiningLog, (string)log?.miner, UInt256.Parse((string)log?.amount),
               UInt256.Parse((string)log?.totalSupply), UInt256.Parse((string)log?.minerBalance), address, sortOrder)
    {
    }

    public StopMiningLog(ulong id, ulong transactionId, Address address, int sortOrder, string details)
        : base(TransactionLogType.StopMiningLog, id, transactionId, address, sortOrder, details)
    {
    }
}