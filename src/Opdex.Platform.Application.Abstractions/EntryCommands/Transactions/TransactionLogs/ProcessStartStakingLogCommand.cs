using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs
{
    public class ProcessStartStakingLogCommand : IRequest<bool>
    {
        public ProcessStartStakingLogCommand(TransactionLog log, ulong blockHeight)
        {
            if (blockHeight < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight));
            }
            
            Log = log as StartStakingLog ?? throw new ArgumentNullException(nameof(log));
            BlockHeight = blockHeight;
        }
        
        public StartStakingLog Log { get; }
        public ulong BlockHeight { get; }
    }
}