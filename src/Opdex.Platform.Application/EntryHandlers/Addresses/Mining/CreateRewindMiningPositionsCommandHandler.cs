using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Addresses;
using Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Mining;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Mining;
using Opdex.Platform.Application.Abstractions.Queries.MiningPools;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Balances;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Addresses.Mining;

public class CreateRewindMiningPositionsCommandHandler : IRequestHandler<CreateRewindMiningPositionsCommand, bool>
{
    private readonly IMediator _mediator;
    private readonly ILogger<CreateRewindMiningPositionsCommandHandler> _logger;

    public CreateRewindMiningPositionsCommandHandler(IMediator mediator, ILogger<CreateRewindMiningPositionsCommandHandler> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(CreateRewindMiningPositionsCommand request, CancellationToken cancellationToken)
    {
        var staleMiningPositions = await _mediator.Send(new RetrieveMiningPositionsByModifiedBlockQuery(request.RewindHeight), cancellationToken);
        var staleCount = staleMiningPositions.Count();

        _logger.LogDebug($"Found {staleCount} stale mining positions.");

        var miningPositionsByPool = staleMiningPositions.GroupBy(position => position.MiningPoolId);

        int refreshFailureCount = 0;

        foreach (var group in miningPositionsByPool)
        {
            var pool = await _mediator.Send(new RetrieveMiningPoolByIdQuery(group.Key));

            var miningPositionChunks = group.Chunk(10);

            foreach (var chunk in miningPositionChunks)
            {
                var callResults = await Task.WhenAll(chunk.Select(async miningPosition =>
                {
                    var balance = await _mediator.Send(new CallCirrusGetMiningBalanceForAddressQuery(pool.Address, miningPosition.Owner, request.RewindHeight));
                    miningPosition.SetBalance(balance, request.RewindHeight);
                    var id = await _mediator.Send(new MakeAddressMiningCommand(miningPosition));
                    return id != 0;
                }));
                refreshFailureCount += callResults.Count(succeeded => !succeeded);
            }
        }

        _logger.LogDebug($"Refreshed {staleCount - refreshFailureCount} mining positions.");

        if (refreshFailureCount > 0) _logger.LogError($"Failed to refresh {refreshFailureCount} stale mining positions.");

        return refreshFailureCount == 0;
    }
}
