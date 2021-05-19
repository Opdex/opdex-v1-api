using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.LiquidityPools
{
    public class ProcessChangeMarketLogCommand : ProcessTransactionLogCommand
    {
        public ProcessChangeMarketLogCommand(TransactionLog log, string sender, ulong blockHeight) : base(sender, blockHeight)
        {
            Log = log as ChangeMarketLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public ChangeMarketLog Log { get; }
    }
}