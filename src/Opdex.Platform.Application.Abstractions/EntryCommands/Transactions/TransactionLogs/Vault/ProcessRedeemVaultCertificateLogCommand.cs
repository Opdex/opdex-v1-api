using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.Vault;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Vault
{
    public class ProcessRedeemVaultCertificateLogCommand : ProcessTransactionLogCommand
    {
        public ProcessRedeemVaultCertificateLogCommand(TransactionLog log, string sender, ulong blockHeight) : base(sender, blockHeight)
        {
            Log = log as RedeemVaultCertificateLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public RedeemVaultCertificateLog Log { get; }
    }
}