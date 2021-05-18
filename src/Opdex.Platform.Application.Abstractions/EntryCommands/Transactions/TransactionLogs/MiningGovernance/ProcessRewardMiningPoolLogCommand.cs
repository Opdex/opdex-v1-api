using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningGovernance;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MiningGovernance
{
    public class ProcessRewardMiningPoolLogCommand : IRequest<bool>
    {
        public ProcessRewardMiningPoolLogCommand(TransactionLog log)
        {
            Log = log as RewardMiningPoolLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public RewardMiningPoolLog Log { get; }
    }
}