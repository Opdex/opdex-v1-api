using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Markets.Permissions;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets.Permissions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Markets.Permissions
{
    public class RetrieveMarketPermissionsByUserQueryHandler : IRequestHandler<RetrieveMarketPermissionsByUserQuery, IEnumerable<MarketPermissionType>>
    {
        private readonly IMediator _mediator;

        public RetrieveMarketPermissionsByUserQueryHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<IEnumerable<MarketPermissionType>> Handle(RetrieveMarketPermissionsByUserQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectMarketPermissionsByUserQuery(request.MarketId, request.User), cancellationToken);
        }
    }
}
