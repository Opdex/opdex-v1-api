using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs
{
    public class ProcessVaultOwnerChangeLogCommand : IRequest<bool>
    {
        public ProcessVaultOwnerChangeLogCommand(TransactionLog log)
        {
            Log = log as VaultOwnerChangeLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public VaultOwnerChangeLog Log { get; }
    }
}