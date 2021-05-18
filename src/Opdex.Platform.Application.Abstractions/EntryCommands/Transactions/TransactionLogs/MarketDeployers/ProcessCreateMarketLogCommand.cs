using System;
using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.MarketDeployers;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MarketDeployers
{
    public class ProcessCreateMarketLogCommand : IRequest<bool>
    {
        public ProcessCreateMarketLogCommand(TransactionLog log, string sender)
        {
            if (!sender.HasValue())
            {
                throw new ArgumentNullException(nameof(sender));
            }
            
            Sender = sender;
            Log = log as CreateMarketLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public CreateMarketLog Log { get; }
        public string Sender { get; }
    }
}