using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Governances;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Governances;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Governances
{
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
            var miningPoolReward = await _mediator.Send(new CallCirrusGetSmartContractPropertyQuery(request.Governance,
                                                                                                    GovernanceConstants.StateKeys.MiningPoolReward,
                                                                                                    SmartContractParameterType.UInt256,
                                                                                                    request.BlockHeight));

            var miningDuration = await _mediator.Send(new CallCirrusGetSmartContractPropertyQuery(request.Governance,
                                                                                                    GovernanceConstants.StateKeys.MiningDuration,
                                                                                                    SmartContractParameterType.UInt64,
                                                                                                    request.BlockHeight));

            var poolsFunded = await _mediator.Send(new CallCirrusGetSmartContractPropertyQuery(request.Governance,
                                                                                               GovernanceConstants.StateKeys.MiningPoolsFunded,
                                                                                               SmartContractParameterType.UInt32,
                                                                                               request.BlockHeight));

            var nominationPeriodEnd = await _mediator.Send(new CallCirrusGetSmartContractPropertyQuery(request.Governance,
                                                                                                       GovernanceConstants.StateKeys.NominationPeriodEnd,
                                                                                                       SmartContractParameterType.UInt64,
                                                                                                       request.BlockHeight));

            var minedToken = await _mediator.Send(new CallCirrusGetSmartContractPropertyQuery(request.Governance,
                                                                                              GovernanceConstants.StateKeys.MinedToken,
                                                                                              SmartContractParameterType.Address,
                                                                                              request.BlockHeight));

            return new MiningGovernanceContractSummary(request.Governance, ulong.Parse(nominationPeriodEnd), uint.Parse(poolsFunded),
                                                       UInt256.Parse(miningPoolReward), ulong.Parse(miningDuration), new Address(minedToken));
        }
    }
}
