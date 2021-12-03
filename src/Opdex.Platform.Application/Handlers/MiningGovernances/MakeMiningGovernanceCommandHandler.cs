using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.MiningGovernances;
using Opdex.Platform.Application.Abstractions.Queries.MiningGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.MiningGovernances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.MiningGovernances;

public class MakeMiningGovernanceCommandHandler : IRequestHandler<MakeMiningGovernanceCommand, ulong>
{
    private readonly IMediator _mediator;

    public MakeMiningGovernanceCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<ulong> Handle(MakeMiningGovernanceCommand request, CancellationToken cancellationToken)
    {
        if (request.Refresh)
        {
            var summary = await _mediator.Send(new RetrieveMiningGovernanceContractSummaryByAddressQuery(request.MiningGovernance.Address,
                                                                                                         request.BlockHeight,
                                                                                                         includeMiningPoolsFunded: request.RefreshMiningPoolsFunded,
                                                                                                         includeNominationPeriodEnd: request.RefreshNominationPeriodEnd,
                                                                                                         includeMiningPoolReward: request.RefreshMiningPoolReward));

            request.MiningGovernance.Update(summary);
        }

        return await _mediator.Send(new PersistMiningGovernanceCommand(request.MiningGovernance), cancellationToken);
    }
}