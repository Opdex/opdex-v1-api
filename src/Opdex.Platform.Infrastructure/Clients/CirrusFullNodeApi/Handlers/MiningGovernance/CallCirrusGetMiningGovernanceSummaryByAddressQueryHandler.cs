using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Domain.Models.ODX;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.MiningGovernance;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.MiningGovernance
{
    public class CallCirrusGetMiningGovernanceSummaryByAddressQueryHandler
        : IRequestHandler<CallCirrusGetMiningGovernanceSummaryByAddressQuery, MiningGovernanceContractSummary>
    {
        private readonly ISmartContractsModule _smartContractsModule;
        private readonly ILogger<CallCirrusGetMiningGovernanceSummaryByAddressQueryHandler> _logger;
        
        public CallCirrusGetMiningGovernanceSummaryByAddressQueryHandler(ISmartContractsModule smartContractsModule, 
            ILogger<CallCirrusGetMiningGovernanceSummaryByAddressQueryHandler> logger)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<MiningGovernanceContractSummary> Handle(CallCirrusGetMiningGovernanceSummaryByAddressQuery request, CancellationToken cancellationToken)
        {
            var localCall = new LocalCallRequestDto(request.Address, request.Address, "get_MinedToken", new string[0]);
            var minedTokenResponse = await _smartContractsModule.LocalCallAsync(localCall, CancellationToken.None);
            var minedToken = minedTokenResponse.Return.ToString();
            
            localCall.MethodName = "get_MiningPoolReward";
            var miningPoolRewardResponse = await _smartContractsModule.LocalCallAsync(localCall, CancellationToken.None);
            var miningPoolReward = miningPoolRewardResponse.Return.ToString();
            
            localCall.MethodName = "get_MiningDuration";
            var miningDurationResponse = await _smartContractsModule.LocalCallAsync(localCall, CancellationToken.None);
            var miningDuration = ulong.Parse(miningDurationResponse.Return.ToString());

            localCall.MethodName = "get_MiningPoolsFunded";
            var miningPoolsFundedResponse = await _smartContractsModule.LocalCallAsync(localCall, CancellationToken.None);
            var miningPoolsFunded = uint.Parse(miningPoolsFundedResponse.Return.ToString());

            localCall.MethodName = "get_NominationPeriodEnd";
            var nominationPeriodEndResponse = await _smartContractsModule.LocalCallAsync(localCall, CancellationToken.None);
            var nominationPeriodEnd = ulong.Parse(nominationPeriodEndResponse.Return.ToString());

            localCall.ContractAddress = minedToken;
            localCall.MethodName = "GetBalance";
            localCall.Parameters = new[] {localCall.ContractAddress.ToSmartContractParameter(SmartContractParameterType.Address)};
            var balanceResponse = await _smartContractsModule.LocalCallAsync(localCall, CancellationToken.None);
            var balance = balanceResponse.Return.ToString();
            
            return new MiningGovernanceContractSummary(request.Address, minedToken, nominationPeriodEnd, balance, miningPoolsFunded, miningPoolReward, miningDuration);
        }
    }
}