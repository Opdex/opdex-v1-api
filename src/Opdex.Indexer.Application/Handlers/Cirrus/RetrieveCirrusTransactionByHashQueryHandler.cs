using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;
using Opdex.Indexer.Application.Abstractions.Queries.Cirrus;
using Opdex.Indexer.Domain.Models;

namespace Opdex.Indexer.Application.Handlers.Cirrus
{
    public class RetrieveCirrusTransactionByHashQueryHandler : IRequestHandler<RetrieveCirrusTransactionByHashQuery, TransactionReceipt>
    {
        private readonly IMediator _mediator;
        
        public RetrieveCirrusTransactionByHashQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task<TransactionReceipt> Handle(RetrieveCirrusTransactionByHashQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = new CallCirrusGetSmartContractTransactionReceiptByTxHashQuery(request.TxHash);
                var transactionReceiptDto = await _mediator.Send(query, cancellationToken);

                return new TransactionReceipt(transactionReceiptDto);
            }
            catch (Exception ex)
            {
                // log - should be a not found expection
                return null;
            }
        }
    }
}