using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.Transactions;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.LiquidityPools
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
            TransactionLogType.CollectStakingRewardsLog
        };

        public readonly IReadOnlyList<SnapshotType> SnapshotTypes = new[]
        {
            SnapshotType.Hourly,
            SnapshotType.Daily
        };
    }
}
