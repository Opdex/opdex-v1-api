using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryQueries.Vaults;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Vaults;

public class GetVaultGovernancesWithFilterQueryHandler : EntryFilterQueryHandler<GetVaultGovernancesWithFilterQuery, VaultGovernancesDto>
{
    private readonly IMediator _mediator;
    private readonly IModelAssembler<VaultGovernance, VaultGovernanceDto> _vaultAssembler;

    public GetVaultGovernancesWithFilterQueryHandler(IMediator mediator, IModelAssembler<VaultGovernance, VaultGovernanceDto> vaultAssembler, ILogger<GetVaultGovernancesWithFilterQueryHandler> logger)
        : base(logger)
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
