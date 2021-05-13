using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs
{
    public class ProcessVaultCertificateRedeemedLogCommand : IRequest<bool>
    {
        public ProcessVaultCertificateRedeemedLogCommand(TransactionLog log)
        {
            Log = log as VaultCertificateRedeemedLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public VaultCertificateRedeemedLog Log { get; }
    }
}