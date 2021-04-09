using System;
using MediatR;
using Opdex.Core.Domain.Models.TransactionLogs;

namespace Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionLogs
{
    public class PersistTransactionLiquidityPoolCreatedLogCommand : IRequest<long>
    {
        public PersistTransactionLiquidityPoolCreatedLogCommand(LiquidityPoolCreatedLog poolCreatedLog)
        {
            LiquidityPoolCreatedLog = poolCreatedLog ?? throw new ArgumentNullException(nameof(poolCreatedLog));
        }
        
        public LiquidityPoolCreatedLog LiquidityPoolCreatedLog { get; }
    }
}