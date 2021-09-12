using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.LiquidityPools;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.LiquidityPools
{
    public class CallCirrusGetOpdexLiquidityPoolByAddressQueryHandler
        : IRequestHandler<CallCirrusGetOpdexLiquidityPoolByAddressQuery, LiquidityPool>
    {
        private readonly ISmartContractsModule _smartContractsModule;

        public CallCirrusGetOpdexLiquidityPoolByAddressQueryHandler(ISmartContractsModule smartContractsModule)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
        }

        public async Task<LiquidityPool> Handle(CallCirrusGetOpdexLiquidityPoolByAddressQuery request, CancellationToken cancellationToken)
        {
            var localCall = new LocalCallRequestDto(request.Address, request.Address, "get_Token", new string[0]);
            var tokenResponse = await _smartContractsModule.LocalCallAsync(localCall, cancellationToken);
            var token = (string)tokenResponse.Return;

            // Todo: Should return a LiquidityPoolContractSummary response
            return token == Address.Empty ? null : new LiquidityPool(request.Address, token, 1);
        }
    }
}
