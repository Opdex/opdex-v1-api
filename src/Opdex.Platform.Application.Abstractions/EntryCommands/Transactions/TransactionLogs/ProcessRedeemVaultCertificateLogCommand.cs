using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs
{
    public class ProcessRedeemVaultCertificateLogCommand : IRequest<bool>
    {
        public ProcessRedeemVaultCertificateLogCommand(TransactionLog log)
        {
            Log = log as RedeemVaultCertificateLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public RedeemVaultCertificateLog Log { get; }
    }
}