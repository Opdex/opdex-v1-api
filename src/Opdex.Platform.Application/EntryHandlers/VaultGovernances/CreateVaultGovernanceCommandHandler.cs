using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;
using Opdex.Platform.Application.Abstractions.EntryCommands.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.VaultGovernances;

public class CreateVaultGovernanceCommandHandler : IRequestHandler<CreateVaultGovernanceCommand, ulong>
{
    private readonly IMediator _mediator;

    public CreateVaultGovernanceCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<ulong> Handle(CreateVaultGovernanceCommand request, CancellationToken cancellationToken)
    {
        var vault = await _mediator.Send(new RetrieveVaultGovernanceByAddressQuery(request.Vault, findOrThrow: false));

        if (vault != null) return vault.Id;

        var vaultGovernanceSummary = await _mediator.Send(new RetrieveVaultGovernanceContractSummaryQuery(request.Vault,
                                                                                                          request.BlockHeight,
                                                                                                          includeVestingDuration:true,
                                                                                                          includeTotalPledgeMinimum: true,
                                                                                                          includeTotalVoteMinimum: true));

        vault = new VaultGovernance(request.Vault, request.TokenId, vaultGovernanceSummary.VestingDuration.GetValueOrDefault(),
                                    vaultGovernanceSummary.TotalPledgeMinimum.GetValueOrDefault(),
                                    vaultGovernanceSummary.TotalVoteMinimum.GetValueOrDefault(), request.BlockHeight);

        return await _mediator.Send(new MakeVaultGovernanceCommand(vault, request.BlockHeight));
    }
}
