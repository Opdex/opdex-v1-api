using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.MarketDeployers;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MarketDeployers;

public class ProcessSetPendingDeployerOwnershipLogCommand : ProcessTransactionLogCommand
{
    public ProcessSetPendingDeployerOwnershipLogCommand(TransactionLog log, Address sender, ulong blockHeight) : base(sender, blockHeight)
    {
        Log = log as SetPendingDeployerOwnershipLog ?? throw new ArgumentNullException(nameof(log));
    }

    public SetPendingDeployerOwnershipLog Log { get; }
}