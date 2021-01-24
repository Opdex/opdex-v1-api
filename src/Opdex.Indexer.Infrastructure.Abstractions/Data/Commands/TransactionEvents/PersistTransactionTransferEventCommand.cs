using System;
using MediatR;
using Opdex.Core.Domain.Models.TransactionReceipt.LogEvents;

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