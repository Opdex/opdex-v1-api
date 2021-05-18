using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.LiquidityPools
{
    public class ProcessSwapLogCommand : IRequest<bool>
    {
        public ProcessSwapLogCommand(TransactionLog log)
        {
            Log = log as SwapLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public SwapLog Log { get; }
    }
}