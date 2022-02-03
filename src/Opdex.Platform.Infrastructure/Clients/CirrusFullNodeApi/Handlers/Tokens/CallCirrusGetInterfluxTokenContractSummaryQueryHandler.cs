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
        try
        {
            var ownerRequest = new LocalCallRequestDto(request.Token, request.Token,
                $"get_{InterfluxTokenConstants.Properties.Owner}", request.BlockHeight);
            var ownerResponse = await _smartContractsModule.LocalCallAsync(ownerRequest, cancellationToken);
            var owner = ownerResponse.DeserializeValue<Address>();

            var nativeChainRequest = new LocalCallRequestDto(request.Token, request.Token,
                $"get_{InterfluxTokenConstants.Properties.NativeChain}", request.BlockHeight);
            var nativeChainResponse = await _smartContractsModule.LocalCallAsync(nativeChainRequest, cancellationToken);
            var nativeChain = nativeChainResponse.DeserializeValue<ExternalChainType>();

            var nativeAddressRequest = new LocalCallRequestDto(request.Token, request.Token,
                $"get_{InterfluxTokenConstants.Properties.NativeAddress}", request.BlockHeight);
            var nativeAddressResponse =
                await _smartContractsModule.LocalCallAsync(nativeAddressRequest, cancellationToken);
            var nativeAddress = nativeAddressResponse.DeserializeValue<string>();

            var summary = new InterfluxTokenContractSummary(request.BlockHeight);
            summary.SetInterfluxDetails(owner, nativeChain, nativeAddress);
            return summary;
        }
        catch (Exception)
        {
            return null;
        }
    }
}
