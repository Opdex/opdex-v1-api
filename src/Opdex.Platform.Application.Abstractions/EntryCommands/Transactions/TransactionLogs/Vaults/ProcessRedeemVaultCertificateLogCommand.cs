using System;
using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.Vaults;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Vaults
{
    public class ProcessRedeemVaultCertificateLogCommand : ProcessTransactionLogCommand
    {
        public ProcessRedeemVaultCertificateLogCommand(TransactionLog log, Address sender, ulong blockHeight) : base(sender, blockHeight)
        {
            Log = log as RedeemVaultCertificateLog ?? throw new ArgumentNullException(nameof(log));
        }

        public RedeemVaultCertificateLog Log { get; }
    }
}
