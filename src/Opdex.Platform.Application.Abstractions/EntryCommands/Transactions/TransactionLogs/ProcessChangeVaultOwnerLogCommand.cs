using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs
{
    public class ProcessChangeVaultOwnerLogCommand : IRequest<bool>
    {
        public ProcessChangeVaultOwnerLogCommand(TransactionLog log)
        {
            Log = log as ChangeVaultOwnerLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public ChangeVaultOwnerLog Log { get; }
    }
}