using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs
{
    public class ProcessMarketChangeLogCommand : IRequest<bool>
    {
        public ProcessMarketChangeLogCommand(TransactionLog log)
        {
            Log = log as MarketChangeLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public MarketChangeLog Log { get; }
    }
}