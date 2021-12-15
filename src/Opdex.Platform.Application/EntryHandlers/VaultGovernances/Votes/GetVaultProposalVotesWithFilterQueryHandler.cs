
using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.VaultGovernances.Votes;
using Opdex.Platform.Application.Abstractions.Models.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Votes;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Proposals;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.VaultGovernances.Votes;

public class GetVaultProposalVotesWithFilterQueryHandler : EntryFilterQueryHandler<GetVaultProposalVotesWithFilterQuery, VaultProposalVotesDto>
{
    private readonly IMediator _mediator;
    private readonly IModelAssembler<VaultProposalVote, VaultProposalVoteDto> _voteAssembler;

    public GetVaultProposalVotesWithFilterQueryHandler(IMediator mediator, IModelAssembler<VaultProposalVote, VaultProposalVoteDto> voteAssembler)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _voteAssembler = voteAssembler ?? throw new ArgumentNullException(nameof(voteAssembler));
    }

    public override async Task<VaultProposalVotesDto> Handle(GetVaultProposalVotesWithFilterQuery request, CancellationToken cancellationToken)
    {
        var vault = await _mediator.Send(new RetrieveVaultGovernanceByAddressQuery(request.Vault, findOrThrow: true), cancellationToken);

        var votes = await _mediator.Send(new RetrieveVaultProposalVotesWithFilterQuery(vault.Id, request.Cursor), cancellationToken);

        var votesResults = votes.ToList();

        var cursorDto = BuildCursorDto(votesResults, request.Cursor, pointerSelector: result => result.Id);

        var assembledResults = await Task.WhenAll(votesResults.Select(vote => _voteAssembler.Assemble(vote)));

        return new VaultProposalVotesDto { Votes = assembledResults, Cursor = cursorDto };
    }
}
