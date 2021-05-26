using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningPools;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MiningPools
{
    public class ProcessStopMiningLogCommand : ProcessTransactionLogCommand
    {
        public ProcessStopMiningLogCommand(TransactionLog log, string sender, ulong blockHeight) : base(sender, blockHeight)
        {
            Log = log as StopMiningLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public StopMiningLog Log { get; }
    }
}