using System;
using MediatR;
using Opdex.Core.Domain.Models.TransactionEvents;

namespace Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionEvents
{
    public class PersistTransactionPoolCreatedEventCommand : IRequest<long>
    {
        public PersistTransactionPoolCreatedEventCommand(PoolCreatedEvent poolCreatedEvent)
        {
            PoolCreatedEvent = poolCreatedEvent ?? throw new ArgumentNullException(nameof(poolCreatedEvent));
        }
        
        public PoolCreatedEvent PoolCreatedEvent { get; }
    }
}