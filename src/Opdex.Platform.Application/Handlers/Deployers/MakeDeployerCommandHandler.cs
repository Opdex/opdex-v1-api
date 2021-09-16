using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Deployers;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Deployers;

namespace Opdex.Platform.Application.Handlers.Deployers
{
    public class MakeDeployerCommandHandler : IRequestHandler<MakeDeployerCommand, long>
    {
        private readonly IMediator _mediator;

        public MakeDeployerCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<long> Handle(MakeDeployerCommand request, CancellationToken cancellationToken)
        {
            // When rewind is true, force the update of updatable properties prior to persistence
            if (request.Rewind)
            {
                var owner = await _mediator.Send(new CallCirrusGetSmartContractPropertyQuery(request.Deployer.Address,
                                                                                             MarketDeployerConstants.StateKeys.Owner,
                                                                                             SmartContractParameterType.Address,
                                                                                             request.BlockHeight));

                request.Deployer.SetOwner(owner.Parse<Address>(), request.BlockHeight);
            }

            // Persist the deployer
            return await _mediator.Send(new PersistDeployerCommand(request.Deployer), cancellationToken);
        }
    }
}
