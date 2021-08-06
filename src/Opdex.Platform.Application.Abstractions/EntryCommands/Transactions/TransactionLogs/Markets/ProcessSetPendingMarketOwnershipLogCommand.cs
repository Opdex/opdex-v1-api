using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.Markets;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Markets
{
    public class ProcessSetPendingMarketOwnershipLogCommand : ProcessTransactionLogCommand
    {
        public ProcessSetPendingMarketOwnershipLogCommand(TransactionLog log, string sender, ulong blockHeight) : base(sender, blockHeight)
        {
            Log = log as SetPendingMarketOwnershipLog ?? throw new ArgumentNullException(nameof(log));
        }

        public SetPendingMarketOwnershipLog Log { get; }
    }
}
