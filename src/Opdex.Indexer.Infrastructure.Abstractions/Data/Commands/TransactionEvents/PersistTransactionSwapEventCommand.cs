using System;
using MediatR;
using Opdex.Indexer.Domain.Models.LogEvents;

namespace Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionEvents
{
    public class PersistTransactionSwapEventCommand : IRequest
    {
        public PersistTransactionSwapEventCommand(SwapEvent swapEvent)
        {
            SwapEvent = swapEvent ?? throw new ArgumentNullException(nameof(swapEvent));
        }
        
        public SwapEvent SwapEvent { get; }
    }
}