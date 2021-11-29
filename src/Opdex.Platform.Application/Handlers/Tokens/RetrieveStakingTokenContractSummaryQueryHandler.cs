using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Constants.SmartContracts.Tokens;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;

namespace Opdex.Platform.Application.Handlers.Tokens
{
    public class RetrieveStakingTokenContractSummaryQueryHandler
        : IRequestHandler<RetrieveStakingTokenContractSummaryQuery, StakingTokenContractSummary>
    {
        private readonly IMediator _mediator;

        public RetrieveStakingTokenContractSummaryQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<StakingTokenContractSummary> Handle(RetrieveStakingTokenContractSummaryQuery request, CancellationToken cancellationToken)
        {
            var summary = new StakingTokenContractSummary(request.BlockHeight);

            if (request.IncludeGenesis)
            {
                var genesis = await _mediator.Send(new CallCirrusGetSmartContractPropertyQuery(request.Token,
                                                                                               StakingTokenConstants.StateKeys.Genesis,
                                                                                               SmartContractParameterType.UInt64,
                                                                                               request.BlockHeight), cancellationToken);
                summary.SetGenesis(genesis);
            }

            if (request.IncludeVault)
            {
                var vault = await _mediator.Send(new CallCirrusGetSmartContractPropertyQuery(request.Token,
                                                                                             StakingTokenConstants.StateKeys.Vault,
                                                                                             SmartContractParameterType.Address,
                                                                                             request.BlockHeight), cancellationToken);
                summary.SetVault(vault);
            }

            if (request.IncludeMiningGovernance)
            {
                var miningGovernance = await _mediator.Send(new CallCirrusGetSmartContractPropertyQuery(request.Token,
                                                                                                  StakingTokenConstants.StateKeys.MiningGovernance,
                                                                                                  SmartContractParameterType.Address,
                                                                                                  request.BlockHeight), cancellationToken);
                summary.SetMiningGovernance(miningGovernance);
            }

            if (request.IncludePeriodIndex)
            {
                var index = await _mediator.Send(new CallCirrusGetSmartContractPropertyQuery(request.Token,
                                                                                             StakingTokenConstants.StateKeys.PeriodIndex,
                                                                                             SmartContractParameterType.UInt32,
                                                                                             request.BlockHeight), cancellationToken);
                summary.SetPeriodIndex(index);
            }

            if (request.IncludePeriodDuration)
            {
                var duration = await _mediator.Send(new CallCirrusGetSmartContractPropertyQuery(request.Token,
                                                                                               StakingTokenConstants.StateKeys.PeriodDuration,
                                                                                               SmartContractParameterType.UInt64,
                                                                                               request.BlockHeight), cancellationToken);
                summary.SetPeriodDuration(duration);
            }

            return summary;
        }
    }
}
