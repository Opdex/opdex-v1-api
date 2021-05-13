using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs
{
    public class ProcessLiquidityPoolCreatedLogCommand : IRequest<bool>
    {
        public ProcessLiquidityPoolCreatedLogCommand(TransactionLog log)
        {
            Log = log as LiquidityPoolCreatedLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public LiquidityPoolCreatedLog Log { get; }
    }
}