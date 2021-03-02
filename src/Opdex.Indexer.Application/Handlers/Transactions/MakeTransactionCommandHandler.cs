using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Core.Application.Abstractions.Queries.Transactions;
using Opdex.Core.Common.Exceptions;
using Opdex.Core.Domain.Models;
using Opdex.Core.Domain.Models.TransactionEvents;
using Opdex.Indexer.Application.Abstractions.Commands.Transactions;
using Opdex.Indexer.Application.Abstractions.Queries.Transactions;
using Opdex.Indexer.Infrastructure.Abstractions.Data.Commands;
using Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionEvents;

namespace Opdex.Indexer.Application.Handlers.Transactions
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
            // Get Transaction - expect not exists
            Transaction transaction;
            try
            {
                transaction = await _mediator.Send(new RetrieveTransactionByHashQuery(request.Transaction.Hash), CancellationToken.None);
            }
            catch (NotFoundException ex)
            {
                transaction = null;
            }

            if (transaction != null)
            {
                // exists already
            }
            
            // Create transaction - return bool
            var transactionId = await _mediator.Send(new PersistTransactionCommand(request.Transaction));

            if (transactionId < 1)
            {
                // Fail or Get transaction? Should that method throw?
                // Would mean the transaction failed to insert
            }
            
            // Get Transaction
            // Set events
            // Make Events
            foreach (var logEvent in request.Transaction.Events)
            {
                await MakeTransactionEvent(logEvent);
            }

            return true;
        }

        private async Task MakeTransactionEvent(TransactionEvent logEvent)
        {
            switch (logEvent)
            {
                case SyncEvent syncEvent:
                    await _mediator.Send(new PersistTransactionSyncEventCommand(syncEvent));
                    break;
                case MintEvent mintEvent:
                    await _mediator.Send(new PersistTransactionMintEventCommand(mintEvent));
                    break;
                case BurnEvent burnEvent:
                    await _mediator.Send(new PersistTransactionBurnEventCommand(burnEvent));
                    break;
                case SwapEvent swapEvent:
                    await _mediator.Send(new PersistTransactionSwapEventCommand(swapEvent));
                    break;
                case ApprovalEvent approvalEvent:
                    await _mediator.Send(new PersistTransactionApprovalEventCommand(approvalEvent));
                    break;
                case TransferEvent transferEvent:
                    await _mediator.Send(new PersistTransactionTransferEventCommand(transferEvent));
                    break;
                case PairCreatedEvent pairCreatedEvent:
                    await _mediator.Send(new PersistTransactionPairCreatedEventCommand(pairCreatedEvent));
                    break;
            }
        }
    }
}