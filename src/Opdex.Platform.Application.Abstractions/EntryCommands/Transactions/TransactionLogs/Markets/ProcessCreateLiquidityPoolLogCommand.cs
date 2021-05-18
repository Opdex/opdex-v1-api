using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.Markets;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Markets
{
    public class ProcessCreateLiquidityPoolLogCommand : IRequest<bool>
    {
        public ProcessCreateLiquidityPoolLogCommand(TransactionLog log)
        {
            Log = log as CreateLiquidityPoolLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public CreateLiquidityPoolLog Log { get; }
    }
}