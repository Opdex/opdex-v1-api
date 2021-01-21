using System;
using MediatR;
using Opdex.Indexer.Domain.Models.LogEvents;

namespace Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionEvents
{
    public class PersistTransactionPairCreatedEventCommand : IRequest
    {
        public PersistTransactionPairCreatedEventCommand(PairCreatedEvent pairCreatedEvent)
        {
            PairCreatedEvent = pairCreatedEvent ?? throw new ArgumentNullException(nameof(pairCreatedEvent));
        }
        
        public PairCreatedEvent PairCreatedEvent { get; }
    }
}