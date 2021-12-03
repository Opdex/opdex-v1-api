using System;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.MarketDeployers;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MarketDeployers;

public class ProcessClaimPendingDeployerOwnershipLogCommand : ProcessTransactionLogCommand
{
    public ProcessClaimPendingDeployerOwnershipLogCommand(TransactionLog log, Address sender, ulong blockHeight) : base(sender, blockHeight)
    {
        Log = log as ClaimPendingDeployerOwnershipLog ?? throw new ArgumentNullException(nameof(log));
    }

    public ClaimPendingDeployerOwnershipLog Log { get; }
}