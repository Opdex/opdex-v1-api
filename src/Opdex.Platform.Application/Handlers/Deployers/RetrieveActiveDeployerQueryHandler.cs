using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Deployers;
using Opdex.Platform.Domain.Models.Deployers;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Deployers;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Deployers
{
    public class RetrieveActiveDeployerQueryHandler : IRequestHandler<RetrieveActiveDeployerQuery, Deployer>
    {
        private readonly IMediator _mediator;

        public RetrieveActiveDeployerQueryHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Deployer> Handle(RetrieveActiveDeployerQuery request, CancellationToken cancellationToken)
        {
            return await _mediator.Send(new SelectActiveDeployerQuery(), cancellationToken);
        }
    }
}
