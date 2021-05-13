using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs
{
    public class ProcessStartMiningLogCommand : IRequest<bool>
    {
        public ProcessStartMiningLogCommand(TransactionLog log)
        {
            Log = log as StartMiningLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public StartMiningLog Log { get; }
    }
}