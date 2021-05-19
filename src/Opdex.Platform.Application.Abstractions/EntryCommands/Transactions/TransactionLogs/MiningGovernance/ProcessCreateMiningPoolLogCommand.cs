using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningGovernance;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MiningGovernance
{
    public class ProcessCreateMiningPoolLogCommand : IRequest<bool>
    {
        public ProcessCreateMiningPoolLogCommand(TransactionLog log, ulong blockHeight)
        {
            if (blockHeight < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight));
            }
            
            Log = log as CreateMiningPoolLog ?? throw new ArgumentNullException(nameof(log));
            BlockHeight = blockHeight;
        }
        
        public CreateMiningPoolLog Log { get; }
        public ulong BlockHeight { get; }
    }
}