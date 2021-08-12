using MediatR;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Tokens
{
    public class CallCirrusGetMiningPoolByTokenQueryHandler : IRequestHandler<CallCirrusGetMiningPoolByTokenQuery, string>
    {
        private readonly ISmartContractsModule _smartContractsModule;

        public CallCirrusGetMiningPoolByTokenQueryHandler(ISmartContractsModule smartContractsModule)
        {
            _smartContractsModule = smartContractsModule;
        }

        public async Task<string> Handle(CallCirrusGetMiningPoolByTokenQuery request, CancellationToken cancellationToken)
        {
            var getTotalSupplyRequest = new LocalCallRequestDto(request.LiquidityPoolAddress, "get_MiningPool", request.BlockHeight);
            var response = await _smartContractsModule.LocalCallAsync(getTotalSupplyRequest, cancellationToken);
            return response.Return.ToString();
        }
    }
}
