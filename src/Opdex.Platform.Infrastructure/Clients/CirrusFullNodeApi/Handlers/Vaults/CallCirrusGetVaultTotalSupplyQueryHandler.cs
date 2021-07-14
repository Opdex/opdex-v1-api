using MediatR;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Vaults;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Vaults
{
    public class CallCirrusGetVaultTotalSupplyQueryHandler : IRequestHandler<CallCirrusGetVaultTotalSupplyQuery, string>
    {
        private readonly ISmartContractsModule _smartContractsModule;

        public CallCirrusGetVaultTotalSupplyQueryHandler(ISmartContractsModule smartContractsModule)
        {
            _smartContractsModule = smartContractsModule;
        }

        public async Task<string> Handle(CallCirrusGetVaultTotalSupplyQuery request, CancellationToken cancellationToken)
        {
            var getTotalSupplyRequest = new LocalCallRequestDto(request.VaultAddress, "get_TotalSupply", request.BlockHeight);
            var response = await _smartContractsModule.LocalCallAsync(getTotalSupplyRequest, cancellationToken);
            return response.Return.ToString();
        }
    }
}