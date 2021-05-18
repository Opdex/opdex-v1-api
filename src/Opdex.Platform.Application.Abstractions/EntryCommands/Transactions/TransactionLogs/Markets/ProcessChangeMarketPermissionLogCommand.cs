using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.Markets;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Markets
{
    public class ProcessChangeMarketPermissionLogCommand : IRequest<bool>
    {
        public ProcessChangeMarketPermissionLogCommand(TransactionLog log)
        {
            Log = log as ChangeMarketPermissionLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public ChangeMarketPermissionLog Log { get; }
    }
}