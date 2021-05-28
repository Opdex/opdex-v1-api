using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningPools;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MiningPools
{
    public class ProcessMineLogCommand : ProcessTransactionLogCommand
    {
        public ProcessMineLogCommand(TransactionLog log, string sender, ulong blockHeight) : base(sender, blockHeight)
        {
            Log = log as MineLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public MineLog Log { get; }
    }
}