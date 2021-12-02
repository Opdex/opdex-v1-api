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
            var totalSupply = await _mediator.Send(new CallCirrusGetSmartContractPropertyQuery(request.VaultGovernance,
                                                                                               VaultGovernanceConstants.StateKeys.TotalSupply,
                                                                                               SmartContractParameterType.UInt256,
                                                                                               request.BlockHeight), cancellationToken);

            summary.SetUnassignedSupply(totalSupply);
        }

        if (request.IncludeProposedSupply)
        {
            var proposedSupply = await _mediator.Send(new CallCirrusGetSmartContractPropertyQuery(request.VaultGovernance,
                                                                                                  VaultGovernanceConstants.StateKeys.TotalProposedAmount,
                                                                                                  SmartContractParameterType.UInt256,
                                                                                                  request.BlockHeight), cancellationToken);

            summary.SetProposedSupply(proposedSupply);
        }

        if (request.IncludePledgeMinimum)
        {
            var pledgeMinimum = await _mediator.Send(new CallCirrusGetSmartContractPropertyQuery(request.VaultGovernance,
                                                                                                 VaultGovernanceConstants.StateKeys.PledgeMinimum,
                                                                                                 SmartContractParameterType.UInt64,
                                                                                                 request.BlockHeight), cancellationToken);

            summary.SetPledgeMinimum(pledgeMinimum);
        }

        if (request.IncludeProposalMinimum)
        {
            var proposalMinimum = await _mediator.Send(new CallCirrusGetSmartContractPropertyQuery(request.VaultGovernance,
                                                                                                   VaultGovernanceConstants.StateKeys.ProposalMinimum,
                                                                                                   SmartContractParameterType.UInt64,
                                                                                                   request.BlockHeight), cancellationToken);

            summary.SetPledgeMinimum(proposalMinimum);
        }

        return summary;
    }
}
