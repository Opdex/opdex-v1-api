using MediatR;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Vaults;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Vaults;

public class CallCirrusGetVaultContractCertificateSummaryByOwnerQueryHandler
    : IRequestHandler<CallCirrusGetVaultContractCertificateSummaryByOwnerQuery, VaultContractCertificateSummary>
{
    private readonly ISmartContractsModule _smartContractsModule;
    private const string MethodName = VaultConstants.Methods.GetCertificate;

    public CallCirrusGetVaultContractCertificateSummaryByOwnerQueryHandler(ISmartContractsModule smartContractsModule)
    {
        _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
    }

    public async Task<VaultContractCertificateSummary> Handle(CallCirrusGetVaultContractCertificateSummaryByOwnerQuery request,
                                                              CancellationToken cancellationToken)
    {
        var parameters = new[] { new SmartContractMethodParameter(request.Owner) };

        var response = await _smartContractsModule.LocalCallAsync(new LocalCallRequestDto(request.Vault, request.Owner,
                                                                                          MethodName, parameters,
                                                                                          request.BlockHeight), cancellationToken);

        return response.DeserializeValue<VaultContractCertificateSummary>();
    }
}
