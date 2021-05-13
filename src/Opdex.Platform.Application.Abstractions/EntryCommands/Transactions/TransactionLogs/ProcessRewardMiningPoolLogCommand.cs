using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs
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