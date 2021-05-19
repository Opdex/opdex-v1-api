using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets;

namespace Opdex.Platform.Application.Handlers.Markets
{
    public class RetrieveMarketByIdQueryHandler : IRequestHandler<RetrieveMarketByIdQuery, Market>
    {
        private readonly IMediator _mediator;
        
        public RetrieveMarketByIdQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public Task<Market> Handle(RetrieveMarketByIdQuery request, CancellationToken cancellationToken)
        {
            var query = new SelectMarketByIdQuery(request.MarketId, request.FindOrThrow);

            return _mediator.Send(query, cancellationToken);
        }
    }
}