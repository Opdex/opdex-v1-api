using System;
using MediatR;
using Opdex.Core.Domain.Models.TransactionEvents;

namespace Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionEvents
{
    public class PersistTransactionSyncEventCommand : IRequest<long>
    {
        public PersistTransactionSyncEventCommand(SyncEvent syncEvent)
        {
            SyncEvent = syncEvent ?? throw new ArgumentNullException(nameof(syncEvent));
        }
        
        public SyncEvent SyncEvent { get; }
    }
}