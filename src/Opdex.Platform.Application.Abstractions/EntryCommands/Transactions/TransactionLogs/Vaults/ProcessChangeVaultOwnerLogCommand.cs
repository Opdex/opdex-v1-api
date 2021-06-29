using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.Vaults;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Vaults
{
    public class ProcessChangeVaultOwnerLogCommand : ProcessTransactionLogCommand
    {
        public ProcessChangeVaultOwnerLogCommand(TransactionLog log, string sender, ulong blockHeight) : base(sender, blockHeight)
        {
            Log = log as ChangeVaultOwnerLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public ChangeVaultOwnerLog Log { get; }
    }
}
