using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Vaults.Proposals;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Proposals;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Vaults.Proposals;

public class GetVaultProposalByVaultAddressAndPublicIdQueryHandler : IRequestHandler<GetVaultProposalByVaultAddressAndPublicIdQuery, VaultProposalDto>
{
    private readonly IMediator _mediator;
    private readonly IModelAssembler<VaultProposal, VaultProposalDto> _proposalAssembler;

    public GetVaultProposalByVaultAddressAndPublicIdQueryHandler(IMediator mediator, IModelAssembler<VaultProposal, VaultProposalDto> proposalAssembler)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _proposalAssembler = proposalAssembler ?? throw new ArgumentNullException(nameof(proposalAssembler));
    }
    public async Task<VaultProposalDto> Handle(GetVaultProposalByVaultAddressAndPublicIdQuery request, CancellationToken cancellationToken)
    {
        var vault = await _mediator.Send(new RetrieveVaultByAddressQuery(request.Vault, findOrThrow: true), cancellationToken);
        var proposal = await _mediator.Send(new RetrieveVaultProposalByVaultIdAndPublicIdQuery(vault.Id, request.PublicProposalId, findOrThrow: true), cancellationToken);
        return await _proposalAssembler.Assemble(proposal);
    }
}
