using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs
{
    public class ProcessPermissionsChangeLogCommand : IRequest<bool>
    {
        public ProcessPermissionsChangeLogCommand(TransactionLog log)
        {
            Log = log as PermissionsChangeLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public PermissionsChangeLog Log { get; }
    }
}