using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;
using Opdex.Platform.Application.Abstractions.EntryCommands.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Votes;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.VaultGovernances.Votes;

public class CreateRewindVaultProposalVotesCommandHandler : IRequestHandler<CreateRewindVaultProposalVotesCommand, bool>
{
    private readonly IMediator _mediator;
    private readonly ILogger<CreateRewindVaultProposalVotesCommandHandler> _logger;

    public CreateRewindVaultProposalVotesCommandHandler(IMediator mediator, ILogger<CreateRewindVaultProposalVotesCommandHandler> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(CreateRewindVaultProposalVotesCommand request, CancellationToken cancellationToken)
    {
        var votes = await _mediator.Send(new RetrieveVaultProposalVotesByModifiedBlockQuery(request.RewindHeight));
        var votesList = votes.ToList();
        var staleCount = votesList.Count;

        _logger.LogDebug($"Found {staleCount} stale vault proposal votes.");

        int refreshFailureCount = 0;

        var voteChunks = votesList.Chunk(5);

        foreach (var chunk in voteChunks)
        {
            await Task.WhenAll(chunk.Select(async vote =>
            {
                // Todo this refreshes the active balance on the vote but not the voted amount that may have changed due to the rewind
                // -- The only way we can access this, is to look back at our transactions
                // -- Consider grouping votes by voter, then for each voter pulling vote transactions, then match to votes
                var voteId = await _mediator.Send(new MakeVaultProposalVoteCommand(vote, request.RewindHeight, refreshBalance: true));

                var voteRefreshed = voteId > 0;

                if (!voteRefreshed) refreshFailureCount++;
            }));
        }

        _logger.LogDebug($"Refreshed {staleCount - refreshFailureCount} vault proposal votes.");

        if (refreshFailureCount > 0) _logger.LogError($"Failed to refresh {refreshFailureCount} stale vault proposal votes.");

        return refreshFailureCount == 0;
    }
}
