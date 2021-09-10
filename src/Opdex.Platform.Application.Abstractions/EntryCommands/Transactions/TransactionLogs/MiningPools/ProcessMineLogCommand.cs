using System;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningPools;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MiningPools
{
    public class ProcessMineLogCommand : ProcessTransactionLogCommand
    {
        public ProcessMineLogCommand(TransactionLog log, Address sender, ulong blockHeight) : base(sender, blockHeight)
        {
            Log = log as MineLog ?? throw new ArgumentNullException(nameof(log));
        }

        public MineLog Log { get; }
    }
}
