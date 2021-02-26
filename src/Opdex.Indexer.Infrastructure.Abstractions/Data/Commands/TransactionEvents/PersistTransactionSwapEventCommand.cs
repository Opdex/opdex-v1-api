using System;
using MediatR;
using Opdex.Core.Domain.Models.Transaction.TransactionEvents;

namespace Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionEvents
{
    public class PersistTransactionSwapEventCommand : IRequest<bool>
    {
        public PersistTransactionSwapEventCommand(SwapEvent swapEvent)
        {
            SwapEvent = swapEvent ?? throw new ArgumentNullException(nameof(swapEvent));
        }
        
        public SwapEvent SwapEvent { get; }
    }
}