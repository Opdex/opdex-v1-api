using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.VaultGovernances.Proposals;
using Opdex.Platform.Application.Abstractions.Models.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Proposals;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.VaultGovernances.Proposals;

public class GetVaultProposalsWithFilterQueryHandler : EntryFilterQueryHandler<GetVaultProposalsWithFilterQuery, VaultProposalsDto>
{
    private readonly IMediator _mediator;
    private readonly IModelAssembler<VaultProposal, VaultProposalDto> _proposalAssembler;

    public GetVaultProposalsWithFilterQueryHandler(IMediator mediator, IModelAssembler<VaultProposal, VaultProposalDto> proposalAssembler)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _proposalAssembler = proposalAssembler ?? throw new ArgumentNullException(nameof(proposalAssembler));
    }

    public override async Task<VaultProposalsDto> Handle(GetVaultProposalsWithFilterQuery request, CancellationToken cancellationToken)
    {
        var vault = await _mediator.Send(new RetrieveVaultGovernanceByAddressQuery(request.Vault, findOrThrow: true), cancellationToken);

        var proposals = await _mediator.Send(new RetrieveVaultProposalsWithFilterQuery(vault.Id, request.Cursor), cancellationToken);

        var proposalsResults = proposals.ToList();

        var cursorDto = BuildCursorDto(proposalsResults, request.Cursor, pointerSelector: result => (result.Expiration, result.PublicId));

        var assembledResults = await Task.WhenAll(proposalsResults.Select(proposal => _proposalAssembler.Assemble(proposal)));

        return new VaultProposalsDto { Proposals = assembledResults, Cursor = cursorDto };
    }
}
