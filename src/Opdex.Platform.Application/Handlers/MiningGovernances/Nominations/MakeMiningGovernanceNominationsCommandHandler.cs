using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.MiningGovernances.Nominations;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.MiningPools;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.MiningGovernances;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Opdex.Platform.Domain.Models.MiningGovernances;
using Opdex.Platform.Application.Abstractions.Commands.MiningGovernances;

namespace Opdex.Platform.Application.Handlers.MiningGovernances.Nominations;

public class MakeMiningGovernanceNominationsCommandHandler : IRequestHandler<MakeMiningGovernanceNominationsCommand, bool>
{
    private readonly IMediator _mediator;

    public MakeMiningGovernanceNominationsCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<bool> Handle(MakeMiningGovernanceNominationsCommand request, CancellationToken cancellationToken)
    {
        var dbNominations = await _mediator.Send(new RetrieveActiveMiningGovernanceNominationsByMiningGovernanceIdQuery(request.MiningGovernance.Id));

        var nominationSummaries = await _mediator.Send(new CallCirrusGetMiningGovernanceNominationsSummaryQuery(request.MiningGovernance.Address, request.BlockHeight));

        var currentNominationIds = await Task.WhenAll(nominationSummaries.Select(async summary =>
        {
            var liquidityPool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(summary.LiquidityPool));
            var miningPool = await _mediator.Send(new RetrieveMiningPoolByLiquidityPoolIdQuery(liquidityPool.Id));

            var nomination = dbNominations.SingleOrDefault(nom => nom.LiquidityPoolId == liquidityPool.Id && nom.MiningPoolId == miningPool.Id);

            nomination ??= await _mediator.Send(new RetrieveMiningGovernanceNominationByLiquidityAndMiningPoolIdQuery(request.MiningGovernance.Id, liquidityPool.Id,
                                                                                                                      miningPool.Id, findOrThrow: false));

            nomination ??= new MiningGovernanceNomination(request.MiningGovernance.Id, liquidityPool.Id, miningPool.Id, true,
                                                          summary.StakingWeight, request.BlockHeight);

            nomination.SetStatus(true, request.BlockHeight);
            nomination.SetWeight(summary.StakingWeight, request.BlockHeight);

            return await _mediator.Send(new MakeMiningGovernanceNominationCommand(nomination));
        }));

        // Each db nomination that is not a current nomination, set is nominated to false.
        var disabledNominations = dbNominations.Where(nom => currentNominationIds.All(currentId => currentId != nom.Id));

        await Task.WhenAll(disabledNominations.Select(nomination =>
        {
            nomination.SetStatus(false, request.BlockHeight);
            return _mediator.Send(new MakeMiningGovernanceNominationCommand(nomination));
        }));

        return true;
    }
}