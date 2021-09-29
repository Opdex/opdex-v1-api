using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Deployers;
using Opdex.Platform.Domain.Models.Deployers;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Deployers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Deployers
{
    public class RetrieveDeployersByModifiedBlockQueryHandler : IRequestHandler<RetrieveDeployersByModifiedBlockQuery, IEnumerable<Deployer>>
    {
        private readonly IMediator _mediator;

        public RetrieveDeployersByModifiedBlockQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<IEnumerable<Deployer>> Handle(RetrieveDeployersByModifiedBlockQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectDeployersByModifiedBlockQuery(request.BlockHeight), cancellationToken);
        }
    }
}
