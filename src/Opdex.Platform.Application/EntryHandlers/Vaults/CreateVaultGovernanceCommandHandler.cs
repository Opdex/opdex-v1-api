using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Vaults;

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

        var vaultSummary = await _mediator.Send(new RetrieveVaultGovernanceContractSummaryQuery(request.Vault,
                                                                                                request.BlockHeight,
                                                                                                includeVestingDuration:true,
                                                                                                includeTotalPledgeMinimum: true,
                                                                                                includeTotalVoteMinimum: true));

        vault = new VaultGovernance(request.Vault, request.TokenId, vaultSummary.VestingDuration.GetValueOrDefault(),
                                    vaultSummary.TotalPledgeMinimum.GetValueOrDefault(),
                                    vaultSummary.TotalVoteMinimum.GetValueOrDefault(), request.BlockHeight);

        return await _mediator.Send(new MakeVaultGovernanceCommand(vault, request.BlockHeight));
    }
}