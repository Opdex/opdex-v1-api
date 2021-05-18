using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.LiquidityPools
{
    public class ProcessBurnLogCommand : IRequest<bool>
    {
        public ProcessBurnLogCommand(TransactionLog log)
        {
            Log = log as BurnLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public BurnLog Log { get; }
    }
}