using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Core.Domain.Models.Transaction;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;
using Opdex.Indexer.Application.Abstractions.Queries.Cirrus;

namespace Opdex.Indexer.Application.Handlers.Cirrus
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
                var txReceipt = await _mediator.Send(query, cancellationToken);

                return txReceipt;
            }
            catch (Exception ex)
            {
                // log - should be a not found expection
                return null;
            }
        }
    }
}