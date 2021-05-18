using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.Markets;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Markets
{
    public class ProcessChangeMarketOwnerLogCommand : IRequest<bool>
    {
        public ProcessChangeMarketOwnerLogCommand(TransactionLog log)
        {
            Log = log as ChangeMarketOwnerLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public ChangeMarketOwnerLog Log { get; }
    }
}