using System;
using MediatR;
using Opdex.Core.Domain.Models.Transaction.TransactionEvents;

namespace Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionEvents
{
    public class PersistTransactionBurnEventCommand : IRequest<bool>
    {
        public PersistTransactionBurnEventCommand(BurnEvent burnEvent)
        {
            BurnEvent = burnEvent ?? throw new ArgumentNullException(nameof(burnEvent));
        }
        
        public BurnEvent BurnEvent { get; }
    }
}