using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;
using Opdex.Indexer.Application.Abstractions.Queries.Cirrus;

namespace Opdex.Indexer.Application.Handlers.Cirrus
{
    public class RetrieveCirrusTransactionByHashQueryHandler : IRequestHandler<RetrieveCirrusTransactionByHashQuery, TransactionReceiptDto>
    {
        private readonly IMediator _mediator;
        
        public RetrieveCirrusTransactionByHashQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task<TransactionReceiptDto> Handle(RetrieveCirrusTransactionByHashQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = new CallCirrusGetSmartContractTransactionReceiptByTxHashQuery(request.TxHash);
                return await _mediator.Send(query, cancellationToken);
            }
            catch (Exception ex)
            {
                // log - should be a not found expection
                return null;
            }
        }
    }
}