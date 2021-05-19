using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.Vault;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Vault
{
    public class ProcessCreateVaultCertificateLogCommand : IRequest<bool>
    {
        public ProcessCreateVaultCertificateLogCommand(TransactionLog log, ulong blockHeight)
        {
            if (blockHeight < 1)
            {
                throw new ArgumentNullException(nameof(blockHeight));
            }
            
            Log = log as CreateVaultCertificateLog ?? throw new ArgumentNullException(nameof(log));
            BlockHeight = blockHeight;
        }
        
        public CreateVaultCertificateLog Log { get; }
        public ulong BlockHeight { get; }
    }
}