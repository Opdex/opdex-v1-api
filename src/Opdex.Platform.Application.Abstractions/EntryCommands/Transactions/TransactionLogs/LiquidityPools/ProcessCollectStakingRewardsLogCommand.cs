using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.LiquidityPools
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