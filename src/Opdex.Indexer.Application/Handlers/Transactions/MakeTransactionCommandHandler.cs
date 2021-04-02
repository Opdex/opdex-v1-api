using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Core.Application.Abstractions.Queries.Transactions;
using Opdex.Core.Common.Exceptions;
using Opdex.Core.Domain.Models;
using Opdex.Core.Domain.Models.TransactionEvents;
using Opdex.Indexer.Application.Abstractions.Commands.Transactions;
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
            var transaction = await GetTransaction(request.Transaction.Hash);

            if (transaction != null) return false;
            
            var transactionId = await _mediator.Send(new PersistTransactionCommand(request.Transaction));

            if (transactionId == null) return false;
            
            foreach (var logEvent in request.Transaction.Events)
            {
                await MakeTransactionEvent(logEvent);
            }

            return true;
        }

        private async Task<Transaction> GetTransaction(string txHash)
        {
            Transaction transaction;
            
            try
            {
                transaction = await _mediator.Send(new RetrieveTransactionByHashQuery(txHash), CancellationToken.None);
            }
            catch (NotFoundException)
            {
                transaction = null;
            }

            return transaction;
        }

        private async Task MakeTransactionEvent(TransactionEvent logEvent)
        {
            var eventId = logEvent switch
            {
                SyncEvent syncEvent => await _mediator.Send(new PersistTransactionSyncEventCommand(syncEvent)),
                MintEvent mintEvent => await _mediator.Send(new PersistTransactionMintEventCommand(mintEvent)),
                BurnEvent burnEvent => await _mediator.Send(new PersistTransactionBurnEventCommand(burnEvent)),
                SwapEvent swapEvent => await _mediator.Send(new PersistTransactionSwapEventCommand(swapEvent)),
                ApprovalEvent approvalEvent => await _mediator.Send(new PersistTransactionApprovalEventCommand(approvalEvent)),
                TransferEvent transferEvent => await _mediator.Send(new PersistTransactionTransferEventCommand(transferEvent)),
                PoolCreatedEvent poolCreatedEvent => await _mediator.Send(new PersistTransactionPoolCreatedEventCommand(poolCreatedEvent)),
                _ => 0
            };

            if (eventId == 0)  return;
            
            var command = new PersistTransactionEventSummaryCommand(logEvent, eventId);
            await _mediator.Send(command);
        }
    }
}