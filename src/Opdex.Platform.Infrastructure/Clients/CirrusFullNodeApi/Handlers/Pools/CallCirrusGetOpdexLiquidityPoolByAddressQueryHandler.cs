using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Pools;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Pools
{
    public class CallCirrusGetOpdexLiquidityPoolByAddressQueryHandler 
        : IRequestHandler<CallCirrusGetOpdexLiquidityPoolByAddressQuery, LiquidityPool>
    {
        private readonly ISmartContractsModule _smartContractsModule;
        private readonly ILogger<CallCirrusGetOpdexLiquidityPoolByAddressQueryHandler> _logger;
        
        public CallCirrusGetOpdexLiquidityPoolByAddressQueryHandler(ISmartContractsModule smartContractsModule, 
            ILogger<CallCirrusGetOpdexLiquidityPoolByAddressQueryHandler> logger)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LiquidityPool> Handle(CallCirrusGetOpdexLiquidityPoolByAddressQuery request, CancellationToken cancellationToken)
        {
            var localCall = new LocalCallRequestDto(request.Address, request.Address, "get_Token", new string[0]);
            var tokenResponse = await _smartContractsModule.LocalCallAsync(localCall, cancellationToken);
            var token = (string)tokenResponse.Return;
            
            return !token.HasValue() ? null : new LiquidityPool(request.Address, token);
        }
    }
}