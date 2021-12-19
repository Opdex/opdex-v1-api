using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Models.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.VaultGovernances;

public class GetVaultGovernancesWithFilterQueryHandler : EntryFilterQueryHandler<GetVaultGovernancesWithFilterQuery, VaultGovernancesDto>
{
    private readonly IMediator _mediator;
    private readonly IModelAssembler<VaultGovernance, VaultGovernanceDto> _vaultAssembler;

    public GetVaultGovernancesWithFilterQueryHandler(IMediator mediator, IModelAssembler<VaultGovernance, VaultGovernanceDto> vaultAssembler)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _vaultAssembler = vaultAssembler ?? throw new ArgumentNullException(nameof(vaultAssembler));
    }

    public override async Task<VaultGovernancesDto> Handle(GetVaultGovernancesWithFilterQuery request, CancellationToken cancellationToken)
    {
        var vaults = await _mediator.Send(new RetrieveVaultGovernancesWithFilterQuery(request.Cursor), cancellationToken);

        var vaultsResults = vaults.ToList();

        var cursorDto = BuildCursorDto(vaultsResults, request.Cursor, pointerSelector: result => result.Id);

        var assembledResults = await Task.WhenAll(vaultsResults.Select(vault => _vaultAssembler.Assemble(vault)));

        return new VaultGovernancesDto() { Vaults = assembledResults, Cursor = cursorDto };
    }
}
