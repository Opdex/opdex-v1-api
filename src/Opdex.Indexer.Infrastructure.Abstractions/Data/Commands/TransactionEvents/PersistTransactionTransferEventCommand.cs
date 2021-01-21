using System;
using MediatR;
using Opdex.Indexer.Domain.Models.LogEvents;

namespace Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionEvents
{
    public class PersistTransactionTransferEventCommand : IRequest
    {
        public PersistTransactionTransferEventCommand(TransferEvent transferEvent)
        {
            TransferEvent = transferEvent ?? throw new ArgumentNullException(nameof(transferEvent));
        }
        
        public TransferEvent TransferEvent { get; }
    }
}