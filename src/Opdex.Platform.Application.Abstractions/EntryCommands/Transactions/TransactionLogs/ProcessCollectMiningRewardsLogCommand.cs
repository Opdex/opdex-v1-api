using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs
{
    public class ProcessCollectMiningRewardsLogCommand : IRequest<bool>
    {
        public ProcessCollectMiningRewardsLogCommand(TransactionLog log)
        {
            Log = log as CollectMiningRewardsLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public CollectMiningRewardsLog Log { get; }
    }
}