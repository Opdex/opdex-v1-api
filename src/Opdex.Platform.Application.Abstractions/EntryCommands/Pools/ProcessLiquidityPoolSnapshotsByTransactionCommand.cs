using System;
using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Pools
{
    public class ProcessLiquidityPoolSnapshotsByTransactionCommand : IRequest<Unit>
    {
        public ProcessLiquidityPoolSnapshotsByTransactionCommand(Transaction transaction)
        {
            Transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        }
        
        public Transaction Transaction { get; }
        
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