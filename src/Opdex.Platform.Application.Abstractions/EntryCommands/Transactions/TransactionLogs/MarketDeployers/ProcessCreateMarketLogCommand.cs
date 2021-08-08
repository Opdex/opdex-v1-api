using System;
using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.MarketDeployers;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MarketDeployers
{
    public class ProcessCreateMarketLogCommand : ProcessTransactionLogCommand
    {
        public ProcessCreateMarketLogCommand(TransactionLog log, string sender, ulong blockHeight) : base(sender, blockHeight)
        {
            Log = log as CreateMarketLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public CreateMarketLog Log { get; }
    }
}