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

public class CallCirrusGetVaultProposalVoteSummaryByProposalIdAndVoterQueryHandler
    : IRequestHandler<CallCirrusGetVaultProposalVoteSummaryByProposalIdAndVoterQuery, VaultProposalVoteSummary>
{
    private readonly ISmartContractsModule _smartContractsModule;
    private const string MethodName = VaultConstants.Methods.GetProposalVote;

    public CallCirrusGetVaultProposalVoteSummaryByProposalIdAndVoterQueryHandler(ISmartContractsModule smartContractsModule)
    {
        _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
    }

    public async Task<VaultProposalVoteSummary> Handle(CallCirrusGetVaultProposalVoteSummaryByProposalIdAndVoterQuery request, CancellationToken cancellationToken)
    {
        var parameters = new[]
        {
            new SmartContractMethodParameter(request.ProposalId),
            new SmartContractMethodParameter(request.Voter)
        };

        var cirrusRequest = new LocalCallRequestDto(request.Vault, request.Vault, MethodName, parameters, request.BlockHeight);

        var response = await _smartContractsModule.LocalCallAsync(cirrusRequest, cancellationToken);

        return response.DeserializeValue<VaultProposalVoteSummary>();
    }
}
