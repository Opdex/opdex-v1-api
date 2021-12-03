using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.VaultGovernances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.VaultGovernances;

public class MakeVaultGovernanceCommandHandler : IRequestHandler<MakeVaultGovernanceCommand, ulong>
{
    private readonly IMediator _mediator;

    public MakeVaultGovernanceCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<ulong> Handle(MakeVaultGovernanceCommand request, CancellationToken cancellationToken)
    {
        if (request.Refresh)
        {
            var summary = await _mediator.Send(new RetrieveVaultGovernanceContractSummaryQuery(request.Vault.Address, request.BlockHeight,
                                                                                               includeUnassignedSupply: request.RefreshUnassignedSupply,
                                                                                               includeProposedSupply: request.RefreshProposedSupply,
                                                                                               includeTotalPledgeMinimum: request.RefreshTotalPledgeMinimum,
                                                                                               includeTotalVoteMinimum: request.RefreshTotalVoteMinimum), CancellationToken.None);

            request.Vault.Update(summary);
        }

        return await _mediator.Send(new PersistVaultGovernanceCommand(request.Vault), CancellationToken.None);
    }
}
