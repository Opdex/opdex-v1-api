using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.LiquidityPools
{
    public class ProcessReservesLogCommand : IRequest<bool>
    {
        public ProcessReservesLogCommand(TransactionLog log)
        {
            Log = log as ReservesLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public ReservesLog Log { get; }
    }
}