using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Core.Domain.Models;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Pairs;

namespace Opdex.Core.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Pairs
{
    public class CallCirrusGetOpdexPairByAddressQueryHandler : IRequestHandler<CallCirrusGetOpdexPairByAddressQuery, Pair>
    {
        private readonly ISmartContractsModule _smartContractsModule;
        private readonly ILogger<CallCirrusGetOpdexPairByAddressQueryHandler> _logger;
        
        public CallCirrusGetOpdexPairByAddressQueryHandler(ISmartContractsModule smartContractsModule, 
            ILogger<CallCirrusGetOpdexPairByAddressQueryHandler> logger)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<Pair> Handle(CallCirrusGetOpdexPairByAddressQuery request, CancellationToken cancellationToken)
        {
            var localCall = new LocalCallRequestDto(request.Address, "Sender", "GetReserves", new string[0]);
            var pairReserves = await _smartContractsModule.LocalCallAsync(localCall, cancellationToken);
            var crsReserves = (ulong)((object[])pairReserves.Return)[0];
            var srcReserves = (string)((object[])pairReserves.Return)[1];

            // Todo: May not be needed or overload Pair constructor
            // Only cares about the internal TokenId at this point
            var token = await _smartContractsModule.GetContractStorageAsync(request.Address, "Token", "string", cancellationToken);

            return new Pair(request.Address, 1L, crsReserves, srcReserves);
        }
    }
}