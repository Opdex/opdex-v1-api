using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions;

namespace Opdex.Platform.Application.Handlers.Transactions
{
    public class RetrieveTransactionByHashQueryHandler : IRequestHandler<RetrieveTransactionByHashQuery, Transaction>
    {
        private readonly IMediator _mediator;
        
        public RetrieveTransactionByHashQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task<Transaction> Handle(RetrieveTransactionByHashQuery request, CancellationToken cancellationToken)
        {
            var transaction = await _mediator.Send(new SelectTransactionByHashQuery(request.Hash), cancellationToken);

            return transaction;
        }
    }
}