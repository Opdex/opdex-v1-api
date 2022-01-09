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

public class CallCirrusGetVaultGovernanceContractCertificateSummaryByOwnerQueryHandler
    : IRequestHandler<CallCirrusGetVaultGovernanceContractCertificateSummaryByOwnerQuery, VaultContractCertificateSummary>
{
    private readonly ISmartContractsModule _smartContractsModule;
    private const string MethodName = VaultGovernanceConstants.Methods.GetCertificate;

    public CallCirrusGetVaultGovernanceContractCertificateSummaryByOwnerQueryHandler(ISmartContractsModule smartContractsModule)
    {
        _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
    }

    public async Task<VaultContractCertificateSummary> Handle(CallCirrusGetVaultGovernanceContractCertificateSummaryByOwnerQuery request,
                                                              CancellationToken cancellationToken)
    {
        var parameters = new[] { new SmartContractMethodParameter(request.Owner) };

        var response = await _smartContractsModule.LocalCallAsync(new LocalCallRequestDto(request.Vault, request.Owner,
                                                                                          MethodName, parameters,
                                                                                          request.BlockHeight), cancellationToken);

        return response.DeserializeValue<VaultContractCertificateSummary>();
    }
}
