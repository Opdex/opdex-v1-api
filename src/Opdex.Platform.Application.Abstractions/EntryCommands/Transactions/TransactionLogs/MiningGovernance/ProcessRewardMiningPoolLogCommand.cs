using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningGovernance;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MiningGovernance
{
    public class ProcessRewardMiningPoolLogCommand : ProcessTransactionLogCommand
    {
        public ProcessRewardMiningPoolLogCommand(TransactionLog log, string sender, ulong blockHeight) : base(sender, blockHeight)
        {
            Log = log as RewardMiningPoolLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public RewardMiningPoolLog Log { get; }
    }
}