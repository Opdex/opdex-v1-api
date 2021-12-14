using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.VaultGovernances.Pledges;
using Opdex.Platform.Application.Abstractions.Models.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Pledges;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Proposals;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.VaultGovernances.Pledges;

public class GetVaultProposalPledgesWithFilterQueryHandler : EntryFilterQueryHandler<GetVaultProposalPledgesWithFilterQuery, VaultProposalPledgesDto>
{
    private readonly IMediator _mediator;
    private readonly IModelAssembler<VaultProposalPledge, VaultProposalPledgeDto> _pledgeAssembler;

    public GetVaultProposalPledgesWithFilterQueryHandler(IMediator mediator, IModelAssembler<VaultProposalPledge, VaultProposalPledgeDto> pledgeAssembler)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _pledgeAssembler = pledgeAssembler ?? throw new ArgumentNullException(nameof(pledgeAssembler));
    }

    public override async Task<VaultProposalPledgesDto> Handle(GetVaultProposalPledgesWithFilterQuery request, CancellationToken cancellationToken)
    {
        var vault = await _mediator.Send(new RetrieveVaultGovernanceByAddressQuery(request.Vault, findOrThrow: true), cancellationToken);
        var proposal = await _mediator.Send(new RetrieveVaultProposalByVaultIdAndPublicIdQuery(vault.Id, request.PublicProposalId, findOrThrow: true), cancellationToken);

        var pledges = await _mediator.Send(new RetrieveVaultProposalPledgesWithFilterQuery(vault.Id, proposal.Id, request.Cursor), cancellationToken);

        var pledgesResults = pledges.ToList();

        var cursorDto = BuildCursorDto(pledgesResults, request.Cursor, pointerSelector: result => result.Id);

        var assembledResults = await Task.WhenAll(pledgesResults.Select(pledge => _pledgeAssembler.Assemble(pledge)));

        return new VaultProposalPledgesDto { Pledges = assembledResults, Cursor = cursorDto };
    }
}
