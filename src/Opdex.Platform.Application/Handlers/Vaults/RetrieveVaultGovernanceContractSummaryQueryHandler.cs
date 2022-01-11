using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Vaults;

public class RetrieveVaultContractSummaryQueryHandler : IRequestHandler<RetrieveVaultContractSummaryQuery, VaultContractSummary>
{
    private readonly IMediator _mediator;

    public RetrieveVaultContractSummaryQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<VaultContractSummary> Handle(RetrieveVaultContractSummaryQuery request, CancellationToken cancellationToken)
    {
        var summary = new VaultContractSummary(request.BlockHeight);

        if (request.IncludeUnassignedSupply)
        {
            SmartContractMethodParameter totalSupply;

            try
            {
                totalSupply = await _mediator.Send(new CallCirrusGetSmartContractPropertyQuery(request.Vault,
                                                                                               VaultConstants.StateKeys.TotalSupply,
                                                                                               SmartContractParameterType.UInt256,
                                                                                               request.BlockHeight), cancellationToken);
            }
            catch
            {
                totalSupply = new SmartContractMethodParameter(0ul);
            }

            summary.SetUnassignedSupply(totalSupply);
        }

        if (request.IncludeProposedSupply)
        {
            SmartContractMethodParameter proposedSupply;

            try
            {
                proposedSupply = await _mediator.Send(new CallCirrusGetSmartContractPropertyQuery(request.Vault,
                                                                                                  VaultConstants.StateKeys.TotalProposedAmount,
                                                                                                  SmartContractParameterType.UInt256,
                                                                                                  request.BlockHeight), cancellationToken);
            }
            catch
            {
                proposedSupply = new SmartContractMethodParameter(0ul);
            }

            summary.SetProposedSupply(proposedSupply);
        }

        if (request.IncludeTotalPledgeMinimum)
        {
            var totalPledgeMinimum = await _mediator.Send(new CallCirrusGetSmartContractPropertyQuery(request.Vault,
                                                                                                      VaultConstants.StateKeys.TotalPledgeMinimum,
                                                                                                      SmartContractParameterType.UInt64,
                                                                                                      request.BlockHeight), cancellationToken);

            summary.SetTotalPledgeMinimum(totalPledgeMinimum);
        }

        if (request.IncludeTotalVoteMinimum)
        {
            var totalVoteMinimum = await _mediator.Send(new CallCirrusGetSmartContractPropertyQuery(request.Vault,
                                                                                                   VaultConstants.StateKeys.TotalVoteMinimum,
                                                                                                   SmartContractParameterType.UInt64,
                                                                                                   request.BlockHeight), cancellationToken);

            summary.SetTotalVoteMinimum(totalVoteMinimum);
        }

        if (request.IncludeVestingDuration)
        {
            var vestingDuration = await _mediator.Send(new CallCirrusGetSmartContractPropertyQuery(request.Vault,
                                                                                                   VaultConstants.StateKeys.VestingDuration,
                                                                                                   SmartContractParameterType.UInt64,
                                                                                                   request.BlockHeight), cancellationToken);

            summary.SetVestingDuration(vestingDuration);
        }

        return summary;
    }
}
