using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.Vaults;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Vaults;

public class ProcessVaultProposalWithdrawPledgeLogCommand : ProcessTransactionLogCommand
{
    public ProcessVaultProposalWithdrawPledgeLogCommand(TransactionLog log, Address sender, ulong blockHeight) : base(sender, blockHeight)
    {
        Log = log as VaultProposalWithdrawPledgeLog ?? throw new ArgumentNullException(nameof(log));
    }

    public VaultProposalWithdrawPledgeLog Log { get; }
}
