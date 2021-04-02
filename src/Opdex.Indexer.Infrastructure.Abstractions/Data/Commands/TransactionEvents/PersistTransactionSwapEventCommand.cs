using System;
using MediatR;
using Opdex.Core.Domain.Models.TransactionEvents;

namespace Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionEvents
{
    public class PersistTransactionSwapEventCommand : IRequest<long>
    {
        public PersistTransactionSwapEventCommand(SwapEvent swapEvent)
        {
            SwapEvent = swapEvent ?? throw new ArgumentNullException(nameof(swapEvent));
        }
        
        public SwapEvent SwapEvent { get; }
    }
}