using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs
{
    public class ProcessMiningPoolCreatedLogCommand : IRequest<bool>
    {
        public ProcessMiningPoolCreatedLogCommand(TransactionLog log)
        {
            Log = log as MiningPoolCreatedLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public MiningPoolCreatedLog Log { get; }
    }
}