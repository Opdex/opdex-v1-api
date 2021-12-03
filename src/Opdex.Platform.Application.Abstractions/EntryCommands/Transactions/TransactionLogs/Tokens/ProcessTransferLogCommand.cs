using System;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Tokens;

public class ProcessTransferLogCommand : ProcessTransactionLogCommand
{
    public ProcessTransferLogCommand(TransactionLog log, Address sender, ulong blockHeight) : base(sender, blockHeight)
    {
        Log = log as TransferLog ?? throw new ArgumentNullException(nameof(log));
    }

    public TransferLog Log { get; }
}