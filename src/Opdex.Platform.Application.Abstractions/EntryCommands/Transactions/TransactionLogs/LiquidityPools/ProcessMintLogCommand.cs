using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.LiquidityPools
{
    public class ProcessMintLogCommand : ProcessTransactionLogCommand
    {
        public ProcessMintLogCommand(TransactionLog log, string sender, ulong blockHeight) : base(sender, blockHeight)
        {
            Log = log as MintLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public MintLog Log { get; }
    }
}