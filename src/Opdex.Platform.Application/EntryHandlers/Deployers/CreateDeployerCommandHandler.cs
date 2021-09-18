using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Deployers;
using Opdex.Platform.Application.Abstractions.EntryCommands.Deployers;
using Opdex.Platform.Application.Abstractions.Queries.Deployers;
using Opdex.Platform.Domain.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Deployers
{
    public class CreateDeployerCommandHandler : IRequestHandler<CreateDeployerCommand, long>
    {
        private readonly IMediator _mediator;

        public CreateDeployerCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<long> Handle(CreateDeployerCommand request, CancellationToken cancellationToken)
        {
            // Get or create the deployer domain model, throw if this is an update and a deployer is not found
            var deployer = await _mediator.Send(new RetrieveDeployerByAddressQuery(request.Deployer, findOrThrow: request.IsUpdate)) ??
                           new Deployer(request.Deployer, request.Owner, isActive: true, request.BlockHeight);

            // Only update when owners are different and request block is higher than deployer modified block
            var hasApplicableUpdates = request.IsUpdate &&
                                       deployer.Owner != request.Owner &&
                                       deployer.ModifiedBlock < request.BlockHeight;

            if (hasApplicableUpdates)
            {
                deployer.SetOwner(request.Owner, request.BlockHeight);
            }

            // No need to persist if nothing has changed
            return hasApplicableUpdates || deployer.Id == 0
                ? await _mediator.Send(new MakeDeployerCommand(deployer, request.BlockHeight))
                : deployer.Id;
        }
    }
}
