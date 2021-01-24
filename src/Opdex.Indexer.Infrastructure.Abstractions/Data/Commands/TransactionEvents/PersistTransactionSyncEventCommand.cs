using System;
using MediatR;
using Opdex.Core.Domain.Models.TransactionReceipt.LogEvents;

namespace Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionEvents
{
    public class PersistTransactionSyncEventCommand : IRequest
    {
        public PersistTransactionSyncEventCommand(SyncEvent syncEvent)
        {
            SyncEvent = syncEvent ?? throw new ArgumentNullException(nameof(syncEvent));
        }
        
        public SyncEvent SyncEvent { get; }
    }
}