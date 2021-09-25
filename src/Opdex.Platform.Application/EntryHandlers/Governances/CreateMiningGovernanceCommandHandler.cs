using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Governances;
using Opdex.Platform.Application.Abstractions.EntryCommands.Governances;
using Opdex.Platform.Application.Abstractions.Queries.Governances;
using Opdex.Platform.Domain.Models.Governances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Governances
{
    public class CreateMiningGovernanceCommandHandler : IRequestHandler<CreateMiningGovernanceCommand, long>
    {
        private readonly IMediator _mediator;

        public CreateMiningGovernanceCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<long> Handle(CreateMiningGovernanceCommand request, CancellationToken cancellationToken)
        {
            var governance = await _mediator.Send(new RetrieveMiningGovernanceByAddressQuery(request.Governance, findOrThrow: false));

            // Return if it already exists
            if (governance != null) return governance.Id;

            var summary = await _mediator.Send(new RetrieveMiningGovernanceContractSummaryByAddressQuery(request.Governance,
                                                                                                         request.BlockHeight,
                                                                                                         includeMiningDuration: true));

            governance = new MiningGovernance(request.Governance,
                                              request.StakingTokenId,
                                              summary.MiningDuration.GetValueOrDefault(),
                                              request.BlockHeight);

            return await _mediator.Send(new MakeMiningGovernanceCommand(governance, request.BlockHeight));
        }
    }
}
