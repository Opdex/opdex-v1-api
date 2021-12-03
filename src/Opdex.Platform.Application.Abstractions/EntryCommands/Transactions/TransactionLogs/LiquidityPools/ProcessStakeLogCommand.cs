using System;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.LiquidityPools;

public class ProcessStakeLogCommand : ProcessTransactionLogCommand
{
    public ProcessStakeLogCommand(TransactionLog log, Address sender, ulong blockHeight) : base(sender, blockHeight)
    {
        Log = log as StakeLog ?? throw new ArgumentNullException(nameof(log));
    }

    public StakeLog Log { get; }
}