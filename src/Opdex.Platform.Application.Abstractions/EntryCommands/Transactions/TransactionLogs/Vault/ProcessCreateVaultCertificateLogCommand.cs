using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.Vault;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Vault
{
    public class ProcessCreateVaultCertificateLogCommand : IRequest<bool>
    {
        public ProcessCreateVaultCertificateLogCommand(TransactionLog log)
        {
            Log = log as CreateVaultCertificateLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public CreateVaultCertificateLog Log { get; }
    }
}