using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.VaultGovernances;

public class RetrieveVaultGovernanceContractSummaryQueryHandler : IRequestHandler<RetrieveVaultGovernanceContractSummaryQuery, VaultGovernanceContractSummary>
{
    private readonly IMediator _mediator;

    public RetrieveVaultGovernanceContractSummaryQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<VaultGovernanceContractSummary> Handle(RetrieveVaultGovernanceContractSummaryQuery request, CancellationToken cancellationToken)
    {
        var summary = new VaultGovernanceContractSummary(request.BlockHeight);

        if (request.IncludeUnassignedSupply)
        {
            var owner = await _mediator.Send(new CallCirrusGetSmartContractPropertyQuery(request.VaultGovernance,
                                                                                         VaultGovernanceConstants.StateKeys.TotalSupply,
                                                                                         SmartContractParameterType.UInt256,
                                                                                         request.BlockHeight), CancellationToken.None);

            summary.SetUnassignedSupply(owner);
        }

        if (request.IncludeProposedSupply)
        {
            var supply = await _mediator.Send(new CallCirrusGetSmartContractPropertyQuery(request.VaultGovernance,
                                                                                          VaultGovernanceConstants.StateKeys.TotalProposedAmount,
                                                                                          SmartContractParameterType.UInt256,
                                                                                          request.BlockHeight), CancellationToken.None);

            summary.SetProposedSupply(supply);
        }

        return summary;
    }
}
