using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;
using Opdex.Platform.Application.Abstractions.EntryCommands.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.VaultGovernances;

public class CreateRewindVaultGovernancesCommandHandler : IRequestHandler<CreateRewindVaultGovernancesCommand, bool>
{
    private readonly IMediator _mediator;
    private readonly ILogger<CreateRewindVaultGovernancesCommandHandler> _logger;

    public CreateRewindVaultGovernancesCommandHandler(IMediator mediator, ILogger<CreateRewindVaultGovernancesCommandHandler> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(CreateRewindVaultGovernancesCommand request, CancellationToken cancellationToken)
    {
        var vaults = await _mediator.Send(new RetrieveVaultGovernancesByModifiedBlockQuery(request.RewindHeight));
        var vaultsList = vaults.ToList();
        var staleCount = vaultsList.Count;

        _logger.LogDebug($"Found {staleCount} stale vaults.");

        int refreshFailureCount = 0;

        var vaultChunks = vaultsList.Chunk(5);

        foreach (var chunk in vaultChunks)
        {
            await Task.WhenAll(chunk.Select(async vault =>
            {
                var vaultId = await _mediator.Send(new MakeVaultGovernanceCommand(vault,
                                                                                  request.RewindHeight,
                                                                                  refreshUnassignedSupply: true,
                                                                                  refreshProposedSupply: true,
                                                                                  refreshTotalPledgeMinimum: true,
                                                                                  refreshTotalVoteMinimum: true));

                var vaultRefreshed = vaultId > 0;

                if (!vaultRefreshed) refreshFailureCount++;
            }));
        }

        _logger.LogDebug($"Refreshed {staleCount - refreshFailureCount} vaults.");

        if (refreshFailureCount > 0) _logger.LogError($"Failed to refresh {refreshFailureCount} stale vaults.");

        return refreshFailureCount == 0;
    }
}
