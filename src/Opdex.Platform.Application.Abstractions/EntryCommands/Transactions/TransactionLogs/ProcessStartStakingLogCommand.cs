using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs
{
    public class ProcessStartStakingLogCommand : IRequest<bool>
    {
        public ProcessStartStakingLogCommand(TransactionLog log)
        {
            Log = log as StartStakingLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public StartStakingLog Log { get; }
    }
}