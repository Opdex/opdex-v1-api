using MediatR;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.VaultGovernances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.VaultGovernances;

public class CallCirrusGetVaultProposalPledgeByProposalIdAndPledgerQueryHandler
    : IRequestHandler<CallCirrusGetVaultProposalPledgeByProposalIdAndPledgerQuery, ulong>
{
    private readonly ISmartContractsModule _smartContractsModule;
    private const string MethodName = VaultGovernanceConstants.Methods.GetProposalPledge;

    public CallCirrusGetVaultProposalPledgeByProposalIdAndPledgerQueryHandler(ISmartContractsModule smartContractsModule)
    {
        _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
    }

    public async Task<ulong> Handle(CallCirrusGetVaultProposalPledgeByProposalIdAndPledgerQuery request, CancellationToken cancellationToken)
    {
        var parameters = new[]
        {
            new SmartContractMethodParameter(request.ProposalId),
            new SmartContractMethodParameter(request.Pledger)
        };

        var cirrusRequest = new LocalCallRequestDto(request.Vault, request.Vault, MethodName, parameters, request.BlockHeight);

        var response = await _smartContractsModule.LocalCallAsync(cirrusRequest, cancellationToken);

        return response.DeserializeValue<ulong>();
    }
}
