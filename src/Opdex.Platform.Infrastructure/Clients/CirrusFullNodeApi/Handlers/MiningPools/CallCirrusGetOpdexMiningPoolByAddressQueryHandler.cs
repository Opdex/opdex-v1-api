using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.MiningPools;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.MiningPools;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.MiningPools
{
    public class CallCirrusGetOpdexMiningPoolByAddressQueryHandler
        : IRequestHandler<CallCirrusGetOpdexMiningPoolByAddressQuery, MiningPoolSmartContractSummary>
    {
        private readonly ISmartContractsModule _smartContractsModule;
        private readonly ILogger<CallCirrusGetOpdexMiningPoolByAddressQueryHandler> _logger;

        public CallCirrusGetOpdexMiningPoolByAddressQueryHandler(ISmartContractsModule smartContractsModule,
            ILogger<CallCirrusGetOpdexMiningPoolByAddressQueryHandler> logger)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<MiningPoolSmartContractSummary> Handle(CallCirrusGetOpdexMiningPoolByAddressQuery request, CancellationToken cancellationToken)
        {
            var localCall = new LocalCallRequestDto(request.Address, request.Address, "get_StakingToken", new string[0]);
            var stakingTokenResponse = await _smartContractsModule.LocalCallAsync(localCall, cancellationToken);
            var stakingToken = stakingTokenResponse.DeserializeValue<Address>();

            localCall.MethodName = "get_RewardRate";
            var rewardRateResponse = await _smartContractsModule.LocalCallAsync(localCall, cancellationToken);
            var rewardRate = rewardRateResponse.DeserializeValue<UInt256>();

            localCall.MethodName = "get_MiningPeriodEndBlock";
            var miningPeriodEndResponse = await _smartContractsModule.LocalCallAsync(localCall, cancellationToken);
            var miningPeriodEnd = miningPeriodEndResponse.DeserializeValue<ulong>();

            return new MiningPoolSmartContractSummary(request.Address, stakingToken, rewardRate, miningPeriodEnd);
        }
    }
}
