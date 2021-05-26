using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.LiquidityPools
{
    public class ProcessStartStakingLogCommand : ProcessTransactionLogCommand
    {
        public ProcessStartStakingLogCommand(TransactionLog log, string sender, ulong blockHeight) : base(sender, blockHeight)
        {
            Log = log as StartStakingLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public StartStakingLog Log { get; }
    }
}