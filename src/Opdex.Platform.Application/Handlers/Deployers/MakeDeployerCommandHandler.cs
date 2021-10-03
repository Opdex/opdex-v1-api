using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Deployers;
using Opdex.Platform.Application.Abstractions.Queries.Deployers;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Deployers;

namespace Opdex.Platform.Application.Handlers.Deployers
{
    public class MakeDeployerCommandHandler : IRequestHandler<MakeDeployerCommand, ulong>
    {
        private readonly IMediator _mediator;

        public MakeDeployerCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<ulong> Handle(MakeDeployerCommand request, CancellationToken cancellationToken)
        {
            // When refresh is true, force the update of updatable properties prior to persistence
            if (request.Refresh)
            {
                var summary = await _mediator.Send(new RetrieveDeployerContractSummaryQuery(request.Deployer.Address,
                                                                                            request.BlockHeight,
                                                                                            includePendingOwner: request.RefreshPendingOwner,
                                                                                            includeOwner: request.RefreshOwner));

                request.Deployer.Update(summary);
            }

            // Persist the deployer
            return await _mediator.Send(new PersistDeployerCommand(request.Deployer), cancellationToken);
        }
    }
}
