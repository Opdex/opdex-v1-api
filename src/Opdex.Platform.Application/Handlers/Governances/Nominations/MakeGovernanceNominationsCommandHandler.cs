using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Governances;
using Opdex.Platform.Application.Abstractions.Queries.Governances.Nominations;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.MiningPools;
using Opdex.Platform.Domain.Models.Governances;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Governances;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Governances.Nominations
{
    public class MakeGovernanceNominationsCommandHandler : IRequestHandler<MakeGovernanceNominationsCommand, bool>
    {
        private readonly IMediator _mediator;

        public MakeGovernanceNominationsCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(MakeGovernanceNominationsCommand request, CancellationToken cancellationToken)
        {
            var dbNominations = await _mediator.Send(new RetrieveActiveGovernanceNominationsByGovernanceIdQuery(request.Governance.Id));

            var nominationSummaries = await _mediator.Send(new CallCirrusGetGovernanceNominationsSummaryQuery(request.Governance.Address, request.BlockHeight));

            var currentNominationIds = await Task.WhenAll(nominationSummaries.Select(async summary =>
            {
                var liquidityPool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(summary.LiquidityPool));
                var miningPool = await _mediator.Send(new RetrieveMiningPoolByLiquidityPoolIdQuery(liquidityPool.Id));

                var nomination = dbNominations.SingleOrDefault(nom => nom.LiquidityPoolId == liquidityPool.Id && nom.MiningPoolId == miningPool.Id);

                nomination ??= await _mediator.Send(new RetrieveMiningGovernanceNominationByLiquidityAndMiningPoolIdQuery(request.Governance.Id,
                                                                                                                          liquidityPool.Id,
                                                                                                                          miningPool.Id));

                nomination ??= new MiningGovernanceNomination(request.Governance.Id, liquidityPool.Id, miningPool.Id, true,
                                                              summary.StakingWeight, request.BlockHeight);

                nomination.SetStatus(true, request.BlockHeight);
                nomination.SetWeight(summary.StakingWeight, request.BlockHeight);

                return await _mediator.Send(new MakeMiningGovernanceNominationCommand(nomination));
            }));

            // Each db nomination that is not a current nomination, set is nominated to false.
            foreach (var dbNomination in dbNominations.Where(nom => currentNominationIds.All(currentId => currentId != nom.Id)))
            {
                dbNomination.SetStatus(false, request.BlockHeight);
                await _mediator.Send(new MakeMiningGovernanceNominationCommand(dbNomination));
            }

            return true;
        }
    }
}
