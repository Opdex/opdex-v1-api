using MediatR;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Vaults;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Vaults;

public class CallCirrusGetVaultContractCertificateSummariesByOwnerQueryHandler
    : IRequestHandler<CallCirrusGetVaultContractCertificateSummariesByOwnerQuery, IEnumerable<VaultContractCertificateSummary>>
{
    private readonly ISmartContractsModule _smartContractsModule;
    private const string MethodName = VaultConstants.Methods.GetCertificates;

    public CallCirrusGetVaultContractCertificateSummariesByOwnerQueryHandler(ISmartContractsModule smartContractsModule)
    {
        _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
    }

    public async Task<IEnumerable<VaultContractCertificateSummary>> Handle(CallCirrusGetVaultContractCertificateSummariesByOwnerQuery request,
                                                                           CancellationToken cancellationToken)
    {
        var parameters = new[] { new SmartContractMethodParameter(request.Owner) };
        var balanceRequest = new LocalCallRequestDto(request.Vault, request.Owner, MethodName, parameters, request.BlockHeight);

        var response = await _smartContractsModule.LocalCallAsync(balanceRequest, cancellationToken);

        return response.DeserializeValue<IEnumerable<VaultContractCertificateSummary>>();
    }
}