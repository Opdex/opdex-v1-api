using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.MarketDeployers;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MarketDeployers
{
    public class ProcessChangeDeployerOwnerLogCommand : IRequest<bool>
    {
        public ProcessChangeDeployerOwnerLogCommand(TransactionLog log)
        {
            Log = log as ChangeDeployerOwnerLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public ChangeDeployerOwnerLog Log { get; }
    }
}