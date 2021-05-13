using System;
using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Pools
{
    public class ProcessLiquidityPoolSnapshotsByTransactionCommand : IRequest<Unit>
    {
        public ProcessLiquidityPoolSnapshotsByTransactionCommand(string txHash)
        {
            if (!txHash.HasValue())
            {
                throw new ArgumentNullException(nameof(txHash));
            }

            TxHash = txHash;
        }
        
        public string TxHash { get; }
        
        public readonly IReadOnlyList<TransactionLogType> PoolSnapshotLogTypes = new[]
        {
            TransactionLogType.ReservesLog,
            TransactionLogType.SwapLog,
            TransactionLogType.StartStakingLog,
            TransactionLogType.StopStakingLog,
            TransactionLogType.CollectStakingRewardsLog,
        };
        
        public readonly IReadOnlyList<SnapshotType> SnapshotTypes = new[]
        {
            SnapshotType.Hourly, 
            SnapshotType.Daily
        };
    }
}