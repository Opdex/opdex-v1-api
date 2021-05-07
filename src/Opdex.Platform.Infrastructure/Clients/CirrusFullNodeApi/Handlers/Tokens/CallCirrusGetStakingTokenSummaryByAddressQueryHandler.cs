using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Tokens
{
    public class CallCirrusGetStakingTokenSummaryByAddressQueryHandler 
        : IRequestHandler<CallCirrusGetStakingTokenSummaryByAddressQuery, StakingTokenContractSummary>
    {
        private readonly ISmartContractsModule _smartContractsModule;
        private readonly ILogger<CallCirrusGetStakingTokenSummaryByAddressQueryHandler> _logger;
        
        public CallCirrusGetStakingTokenSummaryByAddressQueryHandler(ISmartContractsModule smartContractsModule, 
            ILogger<CallCirrusGetStakingTokenSummaryByAddressQueryHandler> logger)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<StakingTokenContractSummary> Handle(CallCirrusGetStakingTokenSummaryByAddressQuery request, CancellationToken cancellationToken)
        {
            var localCall = new LocalCallRequestDto(request.Address, request.Address, "get_MiningGovernance", new string[0]);
            var miningGovernanceResponse = await _smartContractsModule.LocalCallAsync(localCall, CancellationToken.None);
            var miningGovernance = miningGovernanceResponse.Return.ToString();

            localCall.MethodName = "get_PeriodIndex";
            var periodIndexResponse = await _smartContractsModule.LocalCallAsync(localCall, CancellationToken.None);
            var periodIndex = uint.Parse(periodIndexResponse.Return.ToString());

            localCall.MethodName = "get_Owner";
            var ownerResponse = await _smartContractsModule.LocalCallAsync(localCall, CancellationToken.None);
            var owner = ownerResponse.Return.ToString();
            
            localCall.MethodName = "get_Genesis";
            var genesisResponse = await _smartContractsModule.LocalCallAsync(localCall, CancellationToken.None);
            var genesis = ulong.Parse(genesisResponse.Return.ToString());
            
            localCall.MethodName = "get_PeriodDuration";
            var periodDurationResponse = await _smartContractsModule.LocalCallAsync(localCall, CancellationToken.None);
            var periodDuration = ulong.Parse(periodDurationResponse.Return.ToString());

            return new StakingTokenContractSummary(request.Address, miningGovernance, periodIndex, owner, genesis, periodDuration);
        }
    }
}