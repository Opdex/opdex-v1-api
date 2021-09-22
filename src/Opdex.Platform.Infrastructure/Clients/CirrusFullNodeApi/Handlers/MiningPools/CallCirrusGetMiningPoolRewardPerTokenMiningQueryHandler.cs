using MediatR;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.MiningPools;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.MiningPools
{
    public class CallCirrusGetMiningPoolRewardPerTokenMiningQueryHandler
        : IRequestHandler<CallCirrusGetMiningPoolRewardPerTokenMiningQuery, UInt256>
    {
        private readonly ISmartContractsModule _smartContractsModule;
        private const string MethodName = MiningPoolConstants.Methods.GetRewardPerStakedToken;

        public CallCirrusGetMiningPoolRewardPerTokenMiningQueryHandler(ISmartContractsModule smartContractsModule)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
        }

        public async Task<UInt256> Handle(CallCirrusGetMiningPoolRewardPerTokenMiningQuery request, CancellationToken cancellationToken)
        {
            var localCall = new LocalCallRequestDto(request.MiningPool, request.MiningPool, MethodName, request.BlockHeight);
            var response = await _smartContractsModule.LocalCallAsync(localCall, cancellationToken);

            return response.DeserializeValue<UInt256>();
        }
    }
}
