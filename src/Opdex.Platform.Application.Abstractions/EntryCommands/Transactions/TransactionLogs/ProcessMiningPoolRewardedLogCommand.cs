using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs
{
    public class ProcessMiningPoolRewardedLogCommand : IRequest<bool>
    {
        public ProcessMiningPoolRewardedLogCommand(TransactionLog log)
        {
            Log = log as MiningPoolRewardedLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public MiningPoolRewardedLog Log { get; }
    }
}