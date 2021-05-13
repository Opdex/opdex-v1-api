using System.Collections.Generic;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Pools
{
    public class ProcessMiningPoolSnapshotsByTransactionCommand
    {
        public IReadOnlyList<TransactionLogType> PoolSnapshotLogTypes = new[]
        {
            TransactionLogType.MiningPoolRewardedLog,
            TransactionLogType.CollectMiningRewardsLog,
            TransactionLogType.StartMiningLog,
            TransactionLogType.StopMiningLog
        };

        public ProcessMiningPoolSnapshotsByTransactionCommand()
        {
            
        }
    }
}