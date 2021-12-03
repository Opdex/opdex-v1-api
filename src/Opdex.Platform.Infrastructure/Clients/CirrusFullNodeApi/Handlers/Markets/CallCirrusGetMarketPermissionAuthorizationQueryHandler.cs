using MediatR;
using Opdex.Platform.Common.Constants.SmartContracts.Markets;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Markets;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Markets;

public class CallCirrusGetMarketPermissionAuthorizationQueryHandler : IRequestHandler<CallCirrusGetMarketPermissionAuthorizationQuery, bool>
{
    private readonly ISmartContractsModule _smartContractsModule;
    private const string MethodName = StandardMarketConstants.Methods.IsAuthorized;

    public CallCirrusGetMarketPermissionAuthorizationQueryHandler(ISmartContractsModule smartContractsModule)
    {
        _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
    }

    public async Task<bool> Handle(CallCirrusGetMarketPermissionAuthorizationQuery request, CancellationToken cancellationToken)
    {
        var parameters = new[]
        {
            new SmartContractMethodParameter(request.Wallet),
            new SmartContractMethodParameter((byte)request.Permission)
        };

        var localCall = new LocalCallRequestDto(request.Market, request.Market, MethodName, parameters, request.BlockHeight);
        var response = await _smartContractsModule.LocalCallAsync(localCall, cancellationToken);

        return response.DeserializeValue<bool>();
    }
}