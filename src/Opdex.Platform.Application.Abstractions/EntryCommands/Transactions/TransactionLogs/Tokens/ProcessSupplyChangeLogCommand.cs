using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Tokens;

public class ProcessSupplyChangeLogCommand : ProcessTransactionLogCommand
{
    public ProcessSupplyChangeLogCommand(TransactionLog log, Address sender, ulong blockHeight) : base(sender, blockHeight)
    {
        Log = log as SupplyChangeLog ?? throw new ArgumentNullException(nameof(log));
    }

    public SupplyChangeLog Log { get; }
}
