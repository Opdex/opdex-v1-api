using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.Vaults;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Vaults
{
    public class ProcessClaimPendingVaultOwnershipLogCommand : ProcessTransactionLogCommand
    {
        public ProcessClaimPendingVaultOwnershipLogCommand(TransactionLog log, string sender, ulong blockHeight) : base(sender, blockHeight)
        {
            Log = log as ClaimPendingVaultOwnershipLog ?? throw new ArgumentNullException(nameof(log));
        }

        public ClaimPendingVaultOwnershipLog Log { get; }
    }
}
