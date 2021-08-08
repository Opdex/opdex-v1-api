using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Deployers;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Deployers;

namespace Opdex.Platform.Application.Handlers.Deployers
{
    public class RetrieveDeployerByAddressQueryHandler : IRequestHandler<RetrieveDeployerByAddressQuery, Deployer>
    {
        private readonly IMediator _mediator;
        
        public RetrieveDeployerByAddressQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public Task<Deployer> Handle(RetrieveDeployerByAddressQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectDeployerByAddressQuery(request.Address, request.FindOrThrow), cancellationToken);
        }
    }
}