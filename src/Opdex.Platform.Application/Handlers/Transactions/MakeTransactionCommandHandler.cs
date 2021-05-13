using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Transactions.TransactionLogs;

namespace Opdex.Platform.Application.Handlers.Transactions
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
            // Todo: This is also checked by this caller, no needed twice
            var transaction = await GetTransaction(request.Transaction.Hash);

            if (transaction != null) return false;
            
            // Todo: this actually returns out a full Transaction
            var transactionId = await _mediator.Send(new PersistTransactionCommand(request.Transaction), CancellationToken.None);

            if (transactionId == null) return false;
            
            foreach (var log in request.Transaction.Logs)
            {
                // TBH I have no idea how log.TransactionId is being populated correctly here, we don't have it at the request. 
                // Currently a reference bug working in our favor...resolve
                await _mediator.Send(new PersistTransactionLogCommand(log), CancellationToken.None);
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
    }
}