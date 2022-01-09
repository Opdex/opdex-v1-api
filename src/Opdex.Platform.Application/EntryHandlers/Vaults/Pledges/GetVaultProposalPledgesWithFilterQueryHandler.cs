using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryQueries.Vaults.Pledges;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Pledges;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Vaults.Pledges;

public class GetVaultProposalPledgesWithFilterQueryHandler : EntryFilterQueryHandler<GetVaultProposalPledgesWithFilterQuery, VaultProposalPledgesDto>
{
    private readonly IMediator _mediator;
    private readonly IModelAssembler<VaultProposalPledge, VaultProposalPledgeDto> _pledgeAssembler;

    public GetVaultProposalPledgesWithFilterQueryHandler(IMediator mediator, IModelAssembler<VaultProposalPledge, VaultProposalPledgeDto> pledgeAssembler, ILogger<GetVaultProposalPledgesWithFilterQueryHandler> logger)
        : base(logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _pledgeAssembler = pledgeAssembler ?? throw new ArgumentNullException(nameof(pledgeAssembler));
    }

    public override async Task<VaultProposalPledgesDto> Handle(GetVaultProposalPledgesWithFilterQuery request, CancellationToken cancellationToken)
    {
        var vault = await _mediator.Send(new RetrieveVaultGovernanceByAddressQuery(request.Vault, findOrThrow: true), cancellationToken);

        var pledges = await _mediator.Send(new RetrieveVaultProposalPledgesWithFilterQuery(vault.Id, request.Cursor), cancellationToken);

        var pledgesResults = pledges.ToList();

        var cursorDto = BuildCursorDto(pledgesResults, request.Cursor, pointerSelector: result => result.Id);

        var assembledResults = await Task.WhenAll(pledgesResults.Select(pledge => _pledgeAssembler.Assemble(pledge)));

        return new VaultProposalPledgesDto { Pledges = assembledResults, Cursor = cursorDto };
    }
}
