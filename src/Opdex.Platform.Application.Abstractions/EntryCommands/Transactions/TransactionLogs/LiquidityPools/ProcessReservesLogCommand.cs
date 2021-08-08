using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.LiquidityPools
{
    public class ProcessReservesLogCommand : ProcessTransactionLogCommand
    {
        public ProcessReservesLogCommand(TransactionLog log, string sender, ulong blockHeight) : base(sender, blockHeight)
        {
            Log = log as ReservesLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public ReservesLog Log { get; }
    }
}