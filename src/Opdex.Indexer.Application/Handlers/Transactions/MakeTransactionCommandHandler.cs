using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Core.Application.Abstractions.Queries.Transactions;
using Opdex.Core.Common.Exceptions;
using Opdex.Core.Domain.Models;
using Opdex.Core.Domain.Models.TransactionLogs;
using Opdex.Indexer.Application.Abstractions.Commands.Transactions;
using Opdex.Indexer.Infrastructure.Abstractions.Data.Commands;
using Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionLogs;

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
            // Todo: This is also checked by this caller, no needed twice
            var transaction = await GetTransaction(request.Transaction.Hash);

            if (transaction != null) return false;
            
            var transactionId = await _mediator.Send(new PersistTransactionCommand(request.Transaction));

            if (transactionId == null) return false;
            
            foreach (var log in request.Transaction.Logs)
            {
                await _mediator.Send(new PersistTransactionLogCommand(log));
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