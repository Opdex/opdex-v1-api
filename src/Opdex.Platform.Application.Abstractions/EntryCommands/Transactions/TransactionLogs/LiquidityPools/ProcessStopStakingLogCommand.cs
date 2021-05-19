using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.LiquidityPools
{
    public class ProcessStopStakingLogCommand : ProcessTransactionLogCommand
    {
        public ProcessStopStakingLogCommand(TransactionLog log, string sender, ulong blockHeight) : base(sender, blockHeight)
        {
            Log = log as StopStakingLog ?? throw new ArgumentNullException(nameof(log));
        }

        public StopStakingLog Log { get; }
    }
}