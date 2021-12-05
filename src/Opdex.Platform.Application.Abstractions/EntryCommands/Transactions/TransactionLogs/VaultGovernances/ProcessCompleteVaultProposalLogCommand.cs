using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.VaultGovernances;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.VaultGovernances;

public class ProcessCompleteVaultProposalLogCommand : ProcessTransactionLogCommand
{
    public ProcessCompleteVaultProposalLogCommand(TransactionLog log, Address sender, ulong blockHeight) : base(sender, blockHeight)
    {
        Log = log as CompleteVaultProposalLog ?? throw new ArgumentNullException(nameof(log));
    }

    public CompleteVaultProposalLog Log { get; }
}
