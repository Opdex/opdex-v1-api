using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Markets.Permissions;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets.Permissions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Markets.Permissions
{
    public class RetrieveMarketPermissionsByModifiedBlockQueryHandler : IRequestHandler<RetrieveMarketPermissionsByModifiedBlockQuery, IEnumerable<MarketPermission>>
    {
        private readonly IMediator _mediator;

        public RetrieveMarketPermissionsByModifiedBlockQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<IEnumerable<MarketPermission>> Handle(RetrieveMarketPermissionsByModifiedBlockQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectMarketPermissionsByModifiedBlockQuery(request.BlockHeight), cancellationToken);
        }
    }
}
