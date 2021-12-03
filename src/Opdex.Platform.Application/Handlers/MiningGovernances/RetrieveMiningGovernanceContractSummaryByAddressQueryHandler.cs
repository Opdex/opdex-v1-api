using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.MiningGovernances;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Domain.Models.MiningGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.MiningGovernances;

public class RetrieveMiningGovernanceContractSummaryByAddressQueryHandler
    : IRequestHandler<RetrieveMiningGovernanceContractSummaryByAddressQuery, MiningGovernanceContractSummary>
{
    private readonly IMediator _mediator;

    public RetrieveMiningGovernanceContractSummaryByAddressQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<MiningGovernanceContractSummary> Handle(RetrieveMiningGovernanceContractSummaryByAddressQuery request, CancellationToken cancellationToken)
    {
        var summary = new MiningGovernanceContractSummary(request.BlockHeight);

        if (request.IncludeMiningPoolReward)
        {
            var miningPoolReward = await _mediator.Send(new CallCirrusGetSmartContractPropertyQuery(request.MiningGovernance,
                                                                                                    MiningGovernanceConstants.StateKeys.MiningPoolReward,
                                                                                                    SmartContractParameterType.UInt256,
                                                                                                    request.BlockHeight));

            summary.SetMiningPoolReward(miningPoolReward);
        }


        if (request.IncludeMiningPoolsFunded)
        {
            var poolsFunded = await _mediator.Send(new CallCirrusGetSmartContractPropertyQuery(request.MiningGovernance,
                                                                                               MiningGovernanceConstants.StateKeys.MiningPoolsFunded,
                                                                                               SmartContractParameterType.UInt32,
                                                                                               request.BlockHeight));

            summary.SetMiningPoolsFunded(poolsFunded);
        }

        if (request.IncludeMinedToken)
        {
            var minedToken = await _mediator.Send(new CallCirrusGetSmartContractPropertyQuery(request.MiningGovernance,
                                                                                              MiningGovernanceConstants.StateKeys.MinedToken,
                                                                                              SmartContractParameterType.Address,
                                                                                              request.BlockHeight));

            summary.SetMinedToken(minedToken);
        }

        if (request.IncludeMiningDuration)
        {
            var miningDuration = await _mediator.Send(new CallCirrusGetSmartContractPropertyQuery(request.MiningGovernance,
                                                                                                  MiningGovernanceConstants.StateKeys.MiningDuration,
                                                                                                  SmartContractParameterType.UInt64,
                                                                                                  request.BlockHeight));

            summary.SetMiningDuration(miningDuration);
        }

        if (request.IncludeNominationPeriodEnd)
        {
            var nominationPeriodEnd = await _mediator.Send(new CallCirrusGetSmartContractPropertyQuery(request.MiningGovernance,
                                                                                                       MiningGovernanceConstants.StateKeys.NominationPeriodEnd,
                                                                                                       SmartContractParameterType.UInt64,
                                                                                                       request.BlockHeight));

            summary.SetNominationPeriodEnd(nominationPeriodEnd);
        }

        return summary;
    }
}