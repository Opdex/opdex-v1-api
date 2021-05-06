using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Pools;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Pools
{
    public class CallCirrusGetOpdexLiquidityPoolReservesQueryHandler : IRequestHandler<CallCirrusGetOpdexLiquidityPoolReservesQuery, string[]>
    {
        private readonly ISmartContractsModule _smartContractsModule;
        private readonly ILogger<CallCirrusGetOpdexLiquidityPoolReservesQueryHandler> _logger;
        
        public CallCirrusGetOpdexLiquidityPoolReservesQueryHandler(ISmartContractsModule smartContractsModule, 
            ILogger<CallCirrusGetOpdexLiquidityPoolReservesQueryHandler> logger)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string[]> Handle(CallCirrusGetOpdexLiquidityPoolReservesQuery request, CancellationToken cancellationToken)
        {
            var localCall = new LocalCallRequestDto(request.Address, request.Address, "get_Reserves", new string[0]);
            var reservesResponse = await _smartContractsModule.LocalCallAsync(localCall, cancellationToken);
            var reserves = ((JArray)reservesResponse.Return).ToArray();

            return reserves.Any() != true ? new string[0] : reserves.Select(r => r.ToString()).ToArray();
        }
    }
}