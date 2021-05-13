using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs
{
    public class ProcessStopMiningLogCommand : IRequest<bool>
    {
        public ProcessStopMiningLogCommand(TransactionLog log)
        {
            Log = log as StopMiningLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public StopMiningLog Log { get; }
    }
}