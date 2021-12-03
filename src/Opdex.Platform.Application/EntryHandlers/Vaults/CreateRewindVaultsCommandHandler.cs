using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Vaults;

public class CreateRewindVaultsCommandHandler : IRequestHandler<CreateRewindVaultsCommand, bool>
{
    private readonly IMediator _mediator;
    private readonly ILogger<CreateRewindVaultsCommandHandler> _logger;

    public CreateRewindVaultsCommandHandler(IMediator mediator, ILogger<CreateRewindVaultsCommandHandler> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(CreateRewindVaultsCommand request, CancellationToken cancellationToken)
    {
        var vaults = await _mediator.Send(new RetrieveVaultsByModifiedBlockQuery(request.RewindHeight));
        var staleCount = vaults.Count();

        _logger.LogDebug($"Found {staleCount} stale vaults.");

        int refreshFailureCount = 0;

        foreach (var vault in vaults)
        {
            var vaultId = await _mediator.Send(new MakeVaultCommand(vault, request.RewindHeight,
                                                                    refreshPendingOwner: true, refreshOwner: true, refreshSupply: true, refreshGenesis: true));

            var vaultRefreshed = vaultId > 0;

            if (!vaultRefreshed) refreshFailureCount++;
        }

        _logger.LogDebug($"Refreshed {staleCount - refreshFailureCount} vaults.");

        if (refreshFailureCount > 0) _logger.LogError($"Failed to refresh {refreshFailureCount} stale vaults.");

        return refreshFailureCount == 0;
    }
}