using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;
using Opdex.Platform.Application.Abstractions.EntryCommands.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Proposals;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.VaultGovernances.Proposals;

public class CreateRewindVaultProposalsCommandHandler : IRequestHandler<CreateRewindVaultProposalsCommand, bool>
{
    private readonly IMediator _mediator;
    private readonly ILogger<CreateRewindVaultProposalsCommandHandler> _logger;

    public CreateRewindVaultProposalsCommandHandler(IMediator mediator, ILogger<CreateRewindVaultProposalsCommandHandler> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(CreateRewindVaultProposalsCommand request, CancellationToken cancellationToken)
    {
        var proposals = await _mediator.Send(new RetrieveVaultProposalsByModifiedBlockQuery(request.RewindHeight));
        var proposalsList = proposals.ToList();
        var staleCount = proposalsList.Count;

        _logger.LogDebug($"Found {staleCount} stale vault proposals.");

        int refreshFailureCount = 0;

        var proposalChunks = proposalsList.Chunk(5);

        foreach (var chunk in proposalChunks)
        {
            await Task.WhenAll(chunk.Select(async proposal =>
            {
                var proposalId = await _mediator.Send(new MakeVaultProposalCommand(proposal, request.RewindHeight, refreshProposal: true));

                var proposalRefreshed = proposalId > 0;

                if (!proposalRefreshed) refreshFailureCount++;
            }));
        }

        _logger.LogDebug($"Refreshed {staleCount - refreshFailureCount} vault proposals.");

        if (refreshFailureCount > 0) _logger.LogError($"Failed to refresh {refreshFailureCount} stale vault proposals.");

        return refreshFailureCount == 0;
    }
}
