using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs
{
    public class ProcessVaultCertificateUpdatedLogCommand : IRequest<bool>
    {
        public ProcessVaultCertificateUpdatedLogCommand(TransactionLog log)
        {
            Log = log as VaultCertificateUpdatedLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public VaultCertificateUpdatedLog Log { get; }
    }
}