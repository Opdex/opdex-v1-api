using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.Vault;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Vault
{
    public class ProcessRevokeVaultCertificateLogCommand : IRequest<bool>
    {
        public ProcessRevokeVaultCertificateLogCommand(TransactionLog log)
        {
            Log = log as RevokeVaultCertificateLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public RevokeVaultCertificateLog Log { get; }
    }
}