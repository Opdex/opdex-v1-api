using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.MarketDeployers;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MarketDeployers
{
    public class ProcessClaimPendingDeployerOwnershipLogCommand : ProcessTransactionLogCommand
    {
        public ProcessClaimPendingDeployerOwnershipLogCommand(TransactionLog log, string sender, ulong blockHeight) : base(sender, blockHeight)
        {
            Log = log as ClaimPendingDeployerOwnershipLog ?? throw new ArgumentNullException(nameof(log));
        }

        public ClaimPendingDeployerOwnershipLog Log { get; }
    }
}
