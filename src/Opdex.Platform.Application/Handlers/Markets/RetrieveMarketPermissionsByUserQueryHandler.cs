using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Markets
{
    public class RetrieveMarketPermissionsByUserQueryHandler : IRequestHandler<RetrieveMarketPermissionsByUserQuery, IEnumerable<Permissions>>
    {
        private readonly IMediator _mediator;

        public RetrieveMarketPermissionsByUserQueryHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<IEnumerable<Permissions>> Handle(RetrieveMarketPermissionsByUserQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectMarketPermissionsByUserQuery(request.MarketId, request.User), cancellationToken);
        }
    }
}
