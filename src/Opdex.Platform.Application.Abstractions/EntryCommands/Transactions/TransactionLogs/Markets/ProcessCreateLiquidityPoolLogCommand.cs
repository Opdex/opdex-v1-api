using System;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.Markets;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Markets
{
    public class ProcessCreateLiquidityPoolLogCommand : ProcessTransactionLogCommand
    {
        public ProcessCreateLiquidityPoolLogCommand(TransactionLog log, Address sender, ulong blockHeight) : base(sender, blockHeight)
        {
            Log = log as CreateLiquidityPoolLog ?? throw new ArgumentNullException(nameof(log));
        }

        public CreateLiquidityPoolLog Log { get; }
    }
}
