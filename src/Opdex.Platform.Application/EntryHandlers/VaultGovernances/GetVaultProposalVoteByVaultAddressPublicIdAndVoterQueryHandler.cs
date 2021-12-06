using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Models.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Proposals;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Votes;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.VaultGovernances;

public class GetVaultProposalVoteByVaultAddressPublicIdAndVoterQueryHandler : IRequestHandler<GetVaultProposalVoteByVaultAddressPublicIdAndVoterQuery, VaultProposalVoteDto>
{
    private readonly IMediator _mediator;
    private readonly IModelAssembler<VaultProposalVote, VaultProposalVoteDto> _voteAssembler;

    public GetVaultProposalVoteByVaultAddressPublicIdAndVoterQueryHandler(IMediator mediator, IModelAssembler<VaultProposalVote, VaultProposalVoteDto> voteAssembler)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _voteAssembler = voteAssembler ?? throw new ArgumentNullException(nameof(voteAssembler));
    }
    public async Task<VaultProposalVoteDto> Handle(GetVaultProposalVoteByVaultAddressPublicIdAndVoterQuery request, CancellationToken cancellationToken)
    {
        var vault = await _mediator.Send(new RetrieveVaultGovernanceByAddressQuery(request.Vault, findOrThrow: true), cancellationToken);
        var proposal = await _mediator.Send(new RetrieveVaultProposalByVaultIdAndPublicIdQuery(vault.Id, request.PublicProposalId, findOrThrow: true), cancellationToken);
        var vote = await _mediator.Send(new RetrieveVaultProposalVoteByVaultIdAndProposalIdAndVoterQuery(vault.Id, proposal.Id, request.Voter, findOrThrow: true), cancellationToken);
        return await _voteAssembler.Assemble(vote);
    }
}
