using System;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.LiquidityPools
{
    public class ProcessSwapLogCommand : ProcessTransactionLogCommand
    {
        public ProcessSwapLogCommand(TransactionLog log, Address sender, ulong blockHeight) : base(sender, blockHeight)
        {
            Log = log as SwapLog ?? throw new ArgumentNullException(nameof(log));
        }

        public SwapLog Log { get; }
    }
}
