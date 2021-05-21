using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningPools;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MiningPools
{
    public class ProcessStartMiningLogCommand : ProcessTransactionLogCommand
    {
        public ProcessStartMiningLogCommand(TransactionLog log, string sender, ulong blockHeight) : base(sender, blockHeight)
        {
            Log = log as StartMiningLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public StartMiningLog Log { get; }
    }
}