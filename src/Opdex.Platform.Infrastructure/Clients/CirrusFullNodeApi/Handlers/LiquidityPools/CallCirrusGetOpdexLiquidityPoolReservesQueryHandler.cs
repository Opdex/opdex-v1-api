using MediatR;
using Newtonsoft.Json.Linq;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.LiquidityPools;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.LiquidityPools
{
    public class CallCirrusGetOpdexLiquidityPoolReservesQueryHandler : IRequestHandler<CallCirrusGetOpdexLiquidityPoolReservesQuery, UInt256[]>
    {
        private readonly ISmartContractsModule _smartContractsModule;

        public CallCirrusGetOpdexLiquidityPoolReservesQueryHandler(ISmartContractsModule smartContractsModule)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
        }

        public async Task<UInt256[]> Handle(CallCirrusGetOpdexLiquidityPoolReservesQuery request, CancellationToken cancellationToken)
        {
            var localCall = new LocalCallRequestDto(request.Address, request.Address, "get_Reserves", new string[0]);
            var reservesResponse = await _smartContractsModule.LocalCallAsync(localCall, cancellationToken);
            var reserves = ((JArray)reservesResponse.Return).ToArray();

            return reserves.Any() != true ? new UInt256[0] : reserves.Select(r => UInt256.Parse(r.ToString())).ToArray();
        }
    }
}
