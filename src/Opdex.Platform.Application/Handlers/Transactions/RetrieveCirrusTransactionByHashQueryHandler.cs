using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;
using Opdex.Platform.Domain.Models.Transactions;

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
                return await _mediator.Send(new CallCirrusGetTransactionByHashQuery(request.TxHash), cancellationToken);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
