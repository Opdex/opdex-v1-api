using System;
using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.MarketDeployers;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MarketDeployers
{
    public class ProcessCreateMarketLogCommand : IRequest<bool>
    {
        public ProcessCreateMarketLogCommand(TransactionLog log, ulong blockHeight)
        {
            if (blockHeight < 1)
            {
                throw new ArgumentNullException(nameof(blockHeight));
            }
            
            Log = log as CreateMarketLog ?? throw new ArgumentNullException(nameof(log));
            BlockHeight = blockHeight;
        }
        
        public CreateMarketLog Log { get; }
        public ulong BlockHeight { get; }
    }
}