using System;
using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Tokens
{
    public class ProcessTransferLogCommand : ProcessTransactionLogCommand
    {
        public ProcessTransferLogCommand(TransactionLog log, string sender, ulong blockHeight) : base(sender, blockHeight)
        {
            Log = log as TransferLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public TransferLog Log { get; }
    }
}