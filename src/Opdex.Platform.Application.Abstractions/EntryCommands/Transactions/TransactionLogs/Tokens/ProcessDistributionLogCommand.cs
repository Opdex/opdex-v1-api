using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Tokens
{
    public class ProcessDistributionLogCommand : ProcessTransactionLogCommand
    {
        public ProcessDistributionLogCommand(TransactionLog log, string sender, ulong blockHeight) : base(sender, blockHeight)
        {
            Log = log as DistributionLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public DistributionLog Log { get; }
    }
}