using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Opdex.Core.Common.Extensions;
using Opdex.Core.Domain.Models;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Pools;

namespace Opdex.Core.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Pools
{
    public class CallCirrusGetOpdexMiningPoolByAddressQueryHandler 
        : IRequestHandler<CallCirrusGetOpdexMiningPoolByAddressQuery, MiningPool>
    {
        private readonly ISmartContractsModule _smartContractsModule;
        private readonly ILogger<CallCirrusGetOpdexMiningPoolByAddressQueryHandler> _logger;
        
        public CallCirrusGetOpdexMiningPoolByAddressQueryHandler(ISmartContractsModule smartContractsModule, 
            ILogger<CallCirrusGetOpdexMiningPoolByAddressQueryHandler> logger)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<MiningPool> Handle(CallCirrusGetOpdexMiningPoolByAddressQuery request, CancellationToken cancellationToken)
        {
            var localCall = new LocalCallRequestDto(request.Address, request.Address, "get_StakingToken", new string[0]);
            var stakingTokenResponse = await _smartContractsModule.LocalCallAsync(localCall, cancellationToken);
            var stakingToken = (string)stakingTokenResponse.Return;
            if (!stakingToken.HasValue()) return null;
            
            localCall.MethodName = "get_RewardRate";
            var rewardRateResponse = await _smartContractsModule.LocalCallAsync(localCall, cancellationToken);
            var rewardRate = (string)rewardRateResponse.Return;
            if (!rewardRate.HasValue()) return null;
            
            localCall.MethodName = "get_MiningPeriodEndBlock";
            var miningPeriodEndResponse = await _smartContractsModule.LocalCallAsync(localCall, cancellationToken);
            ulong.TryParse(miningPeriodEndResponse.Return.ToString(), out var miningPeriodEnd);
            
            return new MiningPool(request.Address, stakingToken, rewardRate, miningPeriodEnd);
        }
    }
}