using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.MiningPools;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Domain.Models.MiningPools;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.MiningPools;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.MiningPools
{
    public class RetrieveMiningPoolContractSummaryQueryHandler
        : IRequestHandler<RetrieveMiningPoolContractSummaryQuery, MiningPoolContractSummary>
    {
        private readonly IMediator _mediator;

        public RetrieveMiningPoolContractSummaryQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<MiningPoolContractSummary> Handle(RetrieveMiningPoolContractSummaryQuery request, CancellationToken cancellationToken)
        {
            var summary = new MiningPoolContractSummary(request.BlockHeight);

            if (request.IncludeRewardPerBlock)
            {
                var rewardRate = await _mediator.Send(new CallCirrusGetSmartContractPropertyQuery(request.MiningPool,
                                                                                                  MiningPoolConstants.StateKeys.RewardRate,
                                                                                                  SmartContractParameterType.UInt256,
                                                                                                  request.BlockHeight));

                summary.SetRewardRate(rewardRate);
            }

            if (request.IncludeMiningPeriodEndBlock)
            {
                var miningPeriodEndBlock = await _mediator.Send(new CallCirrusGetSmartContractPropertyQuery(request.MiningPool,
                                                                                                            MiningPoolConstants.StateKeys.MiningPeriodEndBlock,
                                                                                                            SmartContractParameterType.UInt64,
                                                                                                            request.BlockHeight));

                summary.SetMiningPeriodEnd(miningPeriodEndBlock);
            }

            if (request.IncludeRewardPerLpt)
            {
                var rewardPerToken = await _mediator.Send(new CallCirrusGetMiningPoolRewardPerTokenMiningQuery(request.MiningPool, request.BlockHeight));

                summary.SetRewardPerLpt(rewardPerToken);
            }

            return summary;
        }
    }
}
