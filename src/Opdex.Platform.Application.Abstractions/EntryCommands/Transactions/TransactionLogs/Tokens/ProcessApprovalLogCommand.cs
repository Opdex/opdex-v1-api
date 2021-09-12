using System;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Tokens
{
    public class ProcessApprovalLogCommand : ProcessTransactionLogCommand
    {
        public ProcessApprovalLogCommand(TransactionLog log, Address sender, ulong blockHeight) : base(sender, blockHeight)
        {
            Log = log as ApprovalLog ?? throw new ArgumentNullException(nameof(log));
        }

        public ApprovalLog Log { get; }
    }
}
