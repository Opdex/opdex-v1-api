using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningPools;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MiningPools
{
    public class ProcessCollectMiningRewardsLogCommand : ProcessTransactionLogCommand
    {
        public ProcessCollectMiningRewardsLogCommand(TransactionLog log, string sender, ulong blockHeight) : base(sender, blockHeight)
        {
            Log = log as CollectMiningRewardsLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public CollectMiningRewardsLog Log { get; }
    }
}