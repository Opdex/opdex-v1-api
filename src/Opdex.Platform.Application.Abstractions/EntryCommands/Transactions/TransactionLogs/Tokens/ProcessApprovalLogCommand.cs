using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Tokens
{
    public class ProcessApprovalLogCommand : ProcessTransactionLogCommand
    {
        public ProcessApprovalLogCommand(TransactionLog log, string sender, ulong blockHeight) : base(sender, blockHeight)
        {
            Log = log as ApprovalLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public ApprovalLog Log { get; }
    }
}