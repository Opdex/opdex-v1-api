using MediatR;
using Opdex.Platform.Common.Constants.SmartContracts.Tokens;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Tokens;

public class CallCirrusGetInterfluxTokenContractSummaryQueryHandler
    : IRequestHandler<CallCirrusGetInterfluxTokenContractSummaryQuery, InterfluxTokenContractSummary>
{
    private readonly ISmartContractsModule _smartContractsModule;

    public CallCirrusGetInterfluxTokenContractSummaryQueryHandler(ISmartContractsModule smartContractsModule)
    {
        _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
    }

    public async Task<InterfluxTokenContractSummary> Handle(CallCirrusGetInterfluxTokenContractSummaryQuery request, CancellationToken cancellationToken)
    {
        var ownerRequest = new LocalCallRequestDto(request.Token, request.Token,
            $"get_{InterfluxTokenConstants.Properties.Owner}", request.BlockHeight);
        var ownerResponse = await _smartContractsModule.LocalCallAsync(ownerRequest, cancellationToken);
        if (!ownerResponse.TryDeserializeValue<Address>(out var owner)) return null;

        var nativeChainRequest = new LocalCallRequestDto(request.Token, request.Token,
            $"get_{InterfluxTokenConstants.Properties.NativeChain}", request.BlockHeight);
        var nativeChainResponse = await _smartContractsModule.LocalCallAsync(nativeChainRequest, cancellationToken);
        if (!nativeChainResponse.TryDeserializeValue<ExternalChainType>(out var nativeChain)) return null;

        var nativeAddressRequest = new LocalCallRequestDto(request.Token, request.Token,
            $"get_{InterfluxTokenConstants.Properties.NativeAddress}", request.BlockHeight);
        var nativeAddressResponse =
            await _smartContractsModule.LocalCallAsync(nativeAddressRequest, cancellationToken);
        if (!nativeAddressResponse.TryDeserializeValue<string>(out var nativeAddress)) return null;

        var summary = new InterfluxTokenContractSummary();
        summary.SetInterfluxDetails(owner, nativeChain, nativeAddress);
        return summary;
    }
}
