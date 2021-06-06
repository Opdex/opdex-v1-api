using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets;

namespace Opdex.Platform.Application.Handlers.Markets
{
    public class RetrieveMarketRouterByAddressQueryHandler : IRequestHandler<RetrieveMarketRouterByAddressQuery, MarketRouter>
    {
        private readonly IMediator _mediator;
        
        public RetrieveMarketRouterByAddressQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public Task<MarketRouter> Handle(RetrieveMarketRouterByAddressQuery request, CancellationToken cancellationToken)
        {
            var query = new SelectMarketRouterByAddressQuery(request.RouterAddress, request.FindOrThrow);

            return _mediator.Send(query, cancellationToken);
        }
    }
}