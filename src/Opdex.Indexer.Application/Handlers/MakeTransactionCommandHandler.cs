using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Core.Domain.Models.TransactionReceipt.LogEvents;
using Opdex.Indexer.Application.Abstractions.Commands;
using Opdex.Indexer.Infrastructure.Abstractions.Data.Commands;

namespace Opdex.Indexer.Application.Handlers
{
    public class MakeTransactionCommandHandler : IRequestHandler<MakeTransactionCommand, bool>
    {
        private readonly IMediator _mediator;
        public MakeTransactionCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task<bool> Handle(MakeTransactionCommand request, CancellationToken cancellationToken)
        {
            // Insert transaction
            var transactionId = await _mediator.Send(new PersistTransactionCommand(request.Transaction));
            
            foreach (var logEvent in request.Transaction.Events)
            {
                logEvent.Event.SetTransactionId(transactionId);
                await MakeTransactionEvent(logEvent.Event);
            }

            return true;
        }
        
        /// <summary>
        /// Placeholder method for different event types to persist.
        /// </summary>
        /// <param name="logEvent">ILogEvent opdex smart contract logged event</param>
        /// <returns></returns>
        private Task MakeTransactionEvent(ILogEvent logEvent)
        {
            // calculate transaction fees
            // index events
            if (logEvent is SyncEvent syncEvent)
            {
                // make sync event command
            }
            else if (logEvent is MintEvent mintEvent)
            {
                // make mint event command
            }
            else if (logEvent is BurnEvent burnEvent)
            {
                // make burn event command
            }
            else if (logEvent is SwapEvent swapEvent)
            {
                // make swap event command
            }
            else if (logEvent is ApprovalEvent approvalEvent)
            {
                // make approval event command
            }
            else if (logEvent is TransferEvent transferEvent)
            {
                // make transfer event command
            }
            else if (logEvent is PairCreatedEvent pairCreatedEvent)
            {
                // make pair created event command
            }

            return Task.CompletedTask;
        }
    }
}