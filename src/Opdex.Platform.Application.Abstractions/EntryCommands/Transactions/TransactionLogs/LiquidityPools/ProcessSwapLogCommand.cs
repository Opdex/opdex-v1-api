using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.LiquidityPools
{
    public class ProcessSwapLogCommand : ProcessTransactionLogCommand
    {
        public ProcessSwapLogCommand(TransactionLog log, string sender, ulong blockHeight) : base(sender, blockHeight)
        {
            Log = log as SwapLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public SwapLog Log { get; }
    }
}