using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Governances;
using Opdex.Platform.Application.Abstractions.EntryCommands.Governances;
using Opdex.Platform.Application.Abstractions.Queries.Governances;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
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
            // Throw if this is an expected update and the record is not found
            var miningGovernance = await _mediator.Send(new RetrieveMiningGovernanceByAddressQuery(request.Governance, findOrThrow: request.IsUpdate));

            var hasApplicableUpdates = request.IsUpdate && miningGovernance.ModifiedBlock < request.BlockHeight;

            if (miningGovernance != null && !hasApplicableUpdates)
            {
                return miningGovernance.Id;
            }

            var summary = await _mediator.Send(new RetrieveMiningGovernanceContractSummaryByAddressQuery(request.Governance, request.BlockHeight));

            if (hasApplicableUpdates)
            {
                miningGovernance.Update(summary, request.BlockHeight);
            }
            else
            {
                var token = await _mediator.Send(new RetrieveTokenByAddressQuery(summary.MinedToken));

                miningGovernance = new MiningGovernance(request.Governance, token.Id, summary.NominationPeriodEnd, summary.MiningDuration,
                                                        summary.MiningPoolsFunded, summary.MiningPoolReward, request.BlockHeight);
            }

            return await _mediator.Send(new MakeMiningGovernanceCommand(miningGovernance, request.BlockHeight, rewind: false));
        }
    }
}
