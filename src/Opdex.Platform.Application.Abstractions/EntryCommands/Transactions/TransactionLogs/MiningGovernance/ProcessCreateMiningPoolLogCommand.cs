using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningGovernance;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MiningGovernance
{
    public class ProcessCreateMiningPoolLogCommand : ProcessTransactionLogCommand
    {
        public ProcessCreateMiningPoolLogCommand(TransactionLog log, string sender, ulong blockHeight) : base(sender, blockHeight)
        {
            Log = log as CreateMiningPoolLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public CreateMiningPoolLog Log { get; }
    }
}