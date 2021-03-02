using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Core.Application.Abstractions.Queries.Transactions;
using Opdex.Core.Domain.Models;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries.Transactions;

namespace Opdex.Core.Application.Handlers.Transactions
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