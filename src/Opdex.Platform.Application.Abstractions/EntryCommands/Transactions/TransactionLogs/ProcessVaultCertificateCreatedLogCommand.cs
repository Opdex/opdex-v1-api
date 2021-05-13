using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs
{
    public class ProcessVaultCertificateCreatedLogCommand : IRequest<bool>
    {
        public ProcessVaultCertificateCreatedLogCommand(TransactionLog log)
        {
            Log = log as VaultCertificateCreatedLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public VaultCertificateCreatedLog Log { get; }
    }
}