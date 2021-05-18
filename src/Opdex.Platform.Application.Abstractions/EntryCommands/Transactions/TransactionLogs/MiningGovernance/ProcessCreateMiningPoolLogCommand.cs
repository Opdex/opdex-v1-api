using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningGovernance;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MiningGovernance
{
    public class ProcessCreateMiningPoolLogCommand : IRequest<bool>
    {
        public ProcessCreateMiningPoolLogCommand(TransactionLog log)
        {
            Log = log as CreateMiningPoolLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public CreateMiningPoolLog Log { get; }
    }
}