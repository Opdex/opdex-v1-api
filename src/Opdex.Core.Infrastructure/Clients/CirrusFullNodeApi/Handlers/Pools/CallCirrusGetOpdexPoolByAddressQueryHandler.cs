using System;
using System.Collections.Generic;
using System.Linq;
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
    public class CallCirrusGetOpdexPoolByAddressQueryHandler : IRequestHandler<CallCirrusGetOpdexPoolByAddressQuery, Pool>
    {
        private readonly ISmartContractsModule _smartContractsModule;
        private readonly ILogger<CallCirrusGetOpdexPoolByAddressQueryHandler> _logger;
        
        public CallCirrusGetOpdexPoolByAddressQueryHandler(ISmartContractsModule smartContractsModule, 
            ILogger<CallCirrusGetOpdexPoolByAddressQueryHandler> logger)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Pool> Handle(CallCirrusGetOpdexPoolByAddressQuery request, CancellationToken cancellationToken)
        {
            var localCall = new LocalCallRequestDto(request.Address, request.Address, "get_Reserves", new string[0]);
            var reservesResponse = await _smartContractsModule.LocalCallAsync(localCall, cancellationToken);
            var reserves = ((JArray)reservesResponse.Return).ToArray();
            
            if (reserves.Any() != true) return null;

            var crsReserves = reserves[0].ToObject<ulong>();
            var srcReserves = reserves[1].ToString();
            
            localCall.MethodName = "get_Token";
            var tokenResponse = await _smartContractsModule.LocalCallAsync(localCall, cancellationToken);
            var token = (string)tokenResponse.Return;
            if (!token.HasValue()) return null;

            return new Pool(request.Address, token, crsReserves, srcReserves);
        }
    }
}