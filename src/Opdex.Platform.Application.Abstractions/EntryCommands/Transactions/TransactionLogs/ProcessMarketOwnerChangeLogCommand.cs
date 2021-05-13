using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs
{
    public class ProcessMarketOwnerChangeLogCommand : IRequest<bool>
    {
        public ProcessMarketOwnerChangeLogCommand(TransactionLog log)
        {
            Log = log as MarketOwnerChangeLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public MarketOwnerChangeLog Log { get; }
    }
}