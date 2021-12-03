using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.Vaults;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Vaults;

public class ProcessSetPendingVaultOwnershipLogCommand : ProcessTransactionLogCommand
{
    public ProcessSetPendingVaultOwnershipLogCommand(TransactionLog log, Address sender, ulong blockHeight) : base(sender, blockHeight)
    {
        Log = log as SetPendingVaultOwnershipLog ?? throw new ArgumentNullException(nameof(log));
    }

    public SetPendingVaultOwnershipLog Log { get; }
}