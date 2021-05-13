using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs
{
    public class ProcessCollectStakingRewardsLogCommand : IRequest<bool>
    {
        public ProcessCollectStakingRewardsLogCommand(TransactionLog log)
        {
            Log = log as CollectStakingRewardsLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public CollectStakingRewardsLog Log { get; }
    }
}