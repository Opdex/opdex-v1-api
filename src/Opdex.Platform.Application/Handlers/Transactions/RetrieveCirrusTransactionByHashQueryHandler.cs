using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;

namespace Opdex.Platform.Application.Handlers.Transactions
{
    public class RetrieveCirrusTransactionByHashQueryHandler : IRequestHandler<RetrieveCirrusTransactionByHashQuery, Transaction>
    {
        private readonly IMediator _mediator;
        
        public RetrieveCirrusTransactionByHashQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task<Transaction> Handle(RetrieveCirrusTransactionByHashQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = new CallCirrusGetTransactionByHashQuery(request.TxHash);
                
                return await _mediator.Send(query, cancellationToken);
            }
            catch (Exception)
            {
                // log - should be a not found expection
                return null;
            }
        }
    }
}