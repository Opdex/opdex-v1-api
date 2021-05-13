using System;
using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs
{
    public class ProcessMarketCreatedLogCommand : IRequest<bool>
    {
        public ProcessMarketCreatedLogCommand(TransactionLog log, string sender)
        {
            if (!sender.HasValue())
            {
                throw new ArgumentNullException(nameof(sender));
            }
            
            Sender = sender;
            Log = log as MarketCreatedLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public MarketCreatedLog Log { get; }
        public string Sender { get; }
    }
}