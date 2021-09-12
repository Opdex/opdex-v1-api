using System;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.MarketDeployers;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MarketDeployers
{
    public class ProcessCreateMarketLogCommand : ProcessTransactionLogCommand
    {
        public ProcessCreateMarketLogCommand(TransactionLog log, Address sender, ulong blockHeight) : base(sender, blockHeight)
        {
            Log = log as CreateMarketLog ?? throw new ArgumentNullException(nameof(log));
        }

        public CreateMarketLog Log { get; }
    }
}
