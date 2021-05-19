using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningGovernance;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MiningGovernance
{
    public class ProcessNominationLogCommand : ProcessTransactionLogCommand
    {
        public ProcessNominationLogCommand(TransactionLog log, string sender, ulong blockHeight) : base(sender, blockHeight)
        {
            Log = log as NominationLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public NominationLog Log { get; }
    }
}