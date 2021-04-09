using System;
using MediatR;
using Opdex.Core.Domain.Models.TransactionLogs;

namespace Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionLogs
{
    public class PersistTransactionTransferLogCommand : IRequest<long>
    {
        public PersistTransactionTransferLogCommand(TransferLog transferLog)
        {
            TransferLog = transferLog ?? throw new ArgumentNullException(nameof(transferLog));
        }
        
        public TransferLog TransferLog { get; }
    }
}