using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningPools;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MiningPools
{
    public class ProcessEnableMiningLogCommand : IRequest<bool>
    {
        public ProcessEnableMiningLogCommand(TransactionLog log)
        {
            Log = log as EnableMiningLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public EnableMiningLog Log { get; }
    }
}