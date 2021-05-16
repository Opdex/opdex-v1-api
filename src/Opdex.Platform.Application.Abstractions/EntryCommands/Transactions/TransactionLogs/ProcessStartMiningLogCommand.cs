using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs
{
    public class ProcessStartMiningLogCommand : IRequest<bool>
    {
        public ProcessStartMiningLogCommand(TransactionLog log, ulong blockHeight)
        {
            if (blockHeight < 1)
            {
                throw new ArgumentNullException(nameof(blockHeight));
            }
            
            Log = log as StartMiningLog ?? throw new ArgumentNullException(nameof(log));
            BlockHeight = blockHeight;
        }
        
        public StartMiningLog Log { get; }
        public ulong BlockHeight { get; }
    }
}