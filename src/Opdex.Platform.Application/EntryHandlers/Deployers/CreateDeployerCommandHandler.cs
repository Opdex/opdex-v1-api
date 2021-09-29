using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Deployers;
using Opdex.Platform.Application.Abstractions.EntryCommands.Deployers;
using Opdex.Platform.Application.Abstractions.Queries.Deployers;
using Opdex.Platform.Domain.Models.Deployers;
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
            var deployer = await _mediator.Send(new RetrieveDeployerByAddressQuery(request.Deployer, findOrThrow: false));

            if (!(deployer is null)) return deployer.Id;

            deployer = new Deployer(request.Deployer, request.Owner, isActive: true, request.BlockHeight);

            return await _mediator.Send(new MakeDeployerCommand(deployer, request.BlockHeight));
        }
    }
}
