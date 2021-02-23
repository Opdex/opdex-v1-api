using System;
using MediatR;
using Opdex.Core.Domain.Models.TransactionReceipt.LogEvents;

namespace Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionEvents
{
    public class PersistTransactionPairCreatedEventCommand : IRequest<bool>
    {
        public PersistTransactionPairCreatedEventCommand(PairCreatedEvent pairCreatedEvent)
        {
            PairCreatedEvent = pairCreatedEvent ?? throw new ArgumentNullException(nameof(pairCreatedEvent));
        }
        
        public PairCreatedEvent PairCreatedEvent { get; }
    }
}