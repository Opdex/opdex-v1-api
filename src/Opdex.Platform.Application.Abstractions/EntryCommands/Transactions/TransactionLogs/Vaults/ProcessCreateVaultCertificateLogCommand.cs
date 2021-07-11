using System;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.Vaults;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Vaults
{
    public class ProcessCreateVaultCertificateLogCommand : ProcessTransactionLogCommand
    {
        public ProcessCreateVaultCertificateLogCommand(TransactionLog log, string sender, ulong blockHeight) : base(sender, blockHeight)
        {
            Log = log as CreateVaultCertificateLog ?? throw new ArgumentNullException(nameof(log));
        }

        public CreateVaultCertificateLog Log { get; }
    }
}
