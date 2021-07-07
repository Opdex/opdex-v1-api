using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Domain.Models.Governances;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Governances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Governances
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
            var minedTokenResponse = await _smartContractsModule.LocalCallAsync(localCall, cancellationToken);
            var minedToken = minedTokenResponse.DeserializeValue<string>();

            localCall.MethodName = "get_MiningPoolReward";
            var miningPoolRewardResponse = await _smartContractsModule.LocalCallAsync(localCall, cancellationToken);
            var miningPoolReward = miningPoolRewardResponse.DeserializeValue<string>();

            localCall.MethodName = "get_MiningDuration";
            var miningDurationResponse = await _smartContractsModule.LocalCallAsync(localCall, cancellationToken);
            var miningDuration = miningDurationResponse.DeserializeValue<ulong>();

            localCall.MethodName = "get_MiningPoolsFunded";
            var miningPoolsFundedResponse = await _smartContractsModule.LocalCallAsync(localCall, cancellationToken);
            var miningPoolsFunded = miningPoolsFundedResponse.DeserializeValue<uint>();

            localCall.MethodName = "get_NominationPeriodEnd";
            var nominationPeriodEndResponse = await _smartContractsModule.LocalCallAsync(localCall, cancellationToken);
            var nominationPeriodEnd = nominationPeriodEndResponse.DeserializeValue<ulong>();

            return new MiningGovernanceContractSummary(request.Address, minedToken, nominationPeriodEnd, miningPoolsFunded, miningPoolReward, miningDuration);
        }
    }
}
