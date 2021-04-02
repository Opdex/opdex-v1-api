using System;
using MediatR;
using Opdex.Core.Domain.Models.TransactionEvents;

namespace Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionEvents
{
    public class PersistTransactionBurnEventCommand : IRequest<long>
    {
        public PersistTransactionBurnEventCommand(BurnEvent burnEvent)
        {
            BurnEvent = burnEvent ?? throw new ArgumentNullException(nameof(burnEvent));
        }
        
        public BurnEvent BurnEvent { get; }
    }
}