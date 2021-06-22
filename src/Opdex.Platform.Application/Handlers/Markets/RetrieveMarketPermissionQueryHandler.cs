using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets;

namespace Opdex.Platform.Application.Handlers.Markets
{
    public class RetrieveMarketPermissionQueryHandler : IRequestHandler<RetrieveMarketPermissionQuery, MarketPermission>
    {
        private readonly IMediator _mediator;

        public RetrieveMarketPermissionQueryHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<MarketPermission> Handle(RetrieveMarketPermissionQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectMarketPermissionQuery(request.MarketId,
                                                                  request.Address,
                                                                  request.Permission,
                                                                  request.FindOrThrow), cancellationToken);
        }
    }
}