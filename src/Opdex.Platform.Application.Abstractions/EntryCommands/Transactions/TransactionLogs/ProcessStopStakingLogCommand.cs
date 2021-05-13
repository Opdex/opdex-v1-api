using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs
{
    public class ProcessStopStakingLogCommand : IRequest<bool>
    {
        public ProcessStopStakingLogCommand(TransactionLog log)
        {
            Log = log as StopStakingLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public StopStakingLog Log { get; }
    }
}