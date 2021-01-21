using System;
using MediatR;
using Opdex.Indexer.Domain.Models.LogEvents;

namespace Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionEvents
{
    public class PersistTransactionBurnEventCommand : IRequest
    {
        public PersistTransactionBurnEventCommand(BurnEvent burnEvent)
        {
            BurnEvent = burnEvent ?? throw new ArgumentNullException(nameof(burnEvent));
        }
        
        public BurnEvent BurnEvent { get; }
    }
}