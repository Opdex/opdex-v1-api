
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryQueries.VaultGovernances.Votes;
using Opdex.Platform.Application.Abstractions.Models.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Votes;
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
    private readonly ILogger<GetVaultProposalVotesWithFilterQueryHandler> _logger;

    public GetVaultProposalVotesWithFilterQueryHandler(IMediator mediator, IModelAssembler<VaultProposalVote, VaultProposalVoteDto> voteAssembler, ILogger<GetVaultProposalVotesWithFilterQueryHandler> logger)
        : base(logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _voteAssembler = voteAssembler ?? throw new ArgumentNullException(nameof(voteAssembler));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override async Task<VaultProposalVotesDto> Handle(GetVaultProposalVotesWithFilterQuery request, CancellationToken cancellationToken)
    {
        var vault = await _mediator.Send(new RetrieveVaultGovernanceByAddressQuery(request.Vault, findOrThrow: true), cancellationToken);

        var votes = await _mediator.Send(new RetrieveVaultProposalVotesWithFilterQuery(vault.Id, request.Cursor), cancellationToken);

        var votesResults = votes.ToList();

        _logger.LogTrace("Retrieved queried votes");

        var cursorDto = BuildCursorDto(votesResults, request.Cursor, pointerSelector: result => result.Id);

        _logger.LogTrace("Returning {ResultCount} results", votesResults.Count);

        var assembledResults = await Task.WhenAll(votesResults.Select(vote => _voteAssembler.Assemble(vote)));

        _logger.LogTrace("Assembled results");

        return new VaultProposalVotesDto { Votes = assembledResults, Cursor = cursorDto };
    }
}
