using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi.Queries;

namespace Opdex.Platform.Application.Handlers
{
    public class RetrieveCmcStraxPriceQueryHandler : IRequestHandler<RetrieveCmcStraxPriceQuery, decimal>
    {
        private readonly IMediator _mediator;
        
        public RetrieveCmcStraxPriceQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task<decimal> Handle(RetrieveCmcStraxPriceQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return await _mediator.Send(new CallCmcGetStraxQuotePriceQuery(), cancellationToken);
            }
            catch (Exception ex)
            {
                return 0m;
            }
        }
    }
}