using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.LiquidityPools
{
    public class ProcessChangeMarketLogCommand : IRequest<bool>
    {
        public ProcessChangeMarketLogCommand(TransactionLog log)
        {
            Log = log as ChangeMarketLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public ChangeMarketLog Log { get; }
    }
}