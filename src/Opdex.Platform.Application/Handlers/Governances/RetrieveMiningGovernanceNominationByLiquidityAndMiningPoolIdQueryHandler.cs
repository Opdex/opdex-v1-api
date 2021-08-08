using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Governances;
using Opdex.Platform.Domain.Models.Governances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Governances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Governances
{
    public class RetrieveMiningGovernanceNominationByLiquidityAndMiningPoolIdQueryHandler
        : IRequestHandler<RetrieveMiningGovernanceNominationByLiquidityAndMiningPoolIdQuery, MiningGovernanceNomination>
    {
        private readonly IMediator _mediator;

        public RetrieveMiningGovernanceNominationByLiquidityAndMiningPoolIdQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<MiningGovernanceNomination> Handle(RetrieveMiningGovernanceNominationByLiquidityAndMiningPoolIdQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectMiningGovernanceNominationByLiquidityAndMiningPoolIdQuery(request.LiquidityPoolId,
                                                                                                      request.MiningPoolId,
                                                                                                      request.FindOrThrow), cancellationToken);
        }
    }
}
