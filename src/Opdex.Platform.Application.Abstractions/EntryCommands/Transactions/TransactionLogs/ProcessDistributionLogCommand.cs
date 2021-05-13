using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs
{
    public class ProcessDistributionLogCommand : IRequest<bool>
    {
        public ProcessDistributionLogCommand(TransactionLog log, ulong blockHeight)
        {
            if (blockHeight < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight));
            }

            BlockHeight = blockHeight;
            Log = log as DistributionLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public DistributionLog Log { get; }
        public ulong BlockHeight { get; }
    }
}