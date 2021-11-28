using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.MiningGovernances;
using Opdex.Platform.Application.Abstractions.EntryCommands.MiningGovernances;
using Opdex.Platform.Application.Abstractions.Queries.MiningGovernances;
using Opdex.Platform.Domain.Models.MiningGovernances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.MiningGovernances
{
    public class CreateMiningGovernanceCommandHandler : IRequestHandler<CreateMiningGovernanceCommand, ulong>
    {
        private readonly IMediator _mediator;

        public CreateMiningGovernanceCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<ulong> Handle(CreateMiningGovernanceCommand request, CancellationToken cancellationToken)
        {
            var miningGovernance = await _mediator.Send(new RetrieveMiningGovernanceByAddressQuery(request.MiningGovernance, findOrThrow: false));

            // Return if it already exists
            if (miningGovernance != null) return miningGovernance.Id;

            var summary = await _mediator.Send(new RetrieveMiningGovernanceContractSummaryByAddressQuery(request.MiningGovernance,
                                                                                                         request.BlockHeight,
                                                                                                         includeMiningDuration: true));

            miningGovernance = new MiningGovernance(request.MiningGovernance,
                                              request.StakingTokenId,
                                              summary.MiningDuration.GetValueOrDefault(),
                                              request.BlockHeight);

            return await _mediator.Send(new MakeMiningGovernanceCommand(miningGovernance, request.BlockHeight));
        }
    }
}
