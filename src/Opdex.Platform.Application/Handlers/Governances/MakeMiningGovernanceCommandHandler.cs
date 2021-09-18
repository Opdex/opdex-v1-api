using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Governances;
using Opdex.Platform.Application.Abstractions.Queries.Governances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Governances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Governances
{
    public class MakeMiningGovernanceCommandHandler : IRequestHandler<MakeMiningGovernanceCommand, long>
    {
        private readonly IMediator _mediator;

        public MakeMiningGovernanceCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<long> Handle(MakeMiningGovernanceCommand request, CancellationToken cancellationToken)
        {
            // Rewind when applicable
            if (request.Rewind)
            {
                var summary = await _mediator.Send(new RetrieveMiningGovernanceContractSummaryByAddressQuery(request.MiningGovernance.Address,
                                                                                                             request.BlockHeight));

                request.MiningGovernance.Update(summary, request.BlockHeight);
            }

            return await _mediator.Send(new PersistMiningGovernanceCommand(request.MiningGovernance), cancellationToken);
        }
    }
}
