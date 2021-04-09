using System;
using MediatR;
using Opdex.Core.Domain.Models.TransactionLogs;

namespace Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionLogs
{
    public class PersistTransactionBurnLogCommand : IRequest<long>
    {
        public PersistTransactionBurnLogCommand(BurnLog burnLog)
        {
            BurnLog = burnLog ?? throw new ArgumentNullException(nameof(burnLog));
        }
        
        public BurnLog BurnLog { get; }
    }
}