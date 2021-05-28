using System.Collections.Generic;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Pools
{
    public class ProcessMiningPoolSnapshotsByTransactionCommand
    {
        public IReadOnlyList<TransactionLogType> PoolSnapshotLogTypes = new[]
        {
            TransactionLogType.EnableMiningLog,
            TransactionLogType.CollectMiningRewardsLog,
            TransactionLogType.MineLog
        };

        public ProcessMiningPoolSnapshotsByTransactionCommand()
        {
            
        }
    }
}