using System;
using MediatR;
using Opdex.Core.Domain.Models.TransactionLogs;

namespace Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionLogs
{
    public class PersistTransactionSwapLogCommand : IRequest<long>
    {
        public PersistTransactionSwapLogCommand(SwapLog swapLog)
        {
            SwapLog = swapLog ?? throw new ArgumentNullException(nameof(swapLog));
        }
        
        public SwapLog SwapLog { get; }
    }
}