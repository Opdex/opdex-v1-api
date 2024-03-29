using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Markets;
using Opdex.Platform.Application.Abstractions.EntryCommands.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Common.Extensions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Markets;

public class CreateRewindMarketsCommandHandler : IRequestHandler<CreateRewindMarketsCommand, bool>
{
    private readonly IMediator _mediator;
    private readonly ILogger<CreateRewindMarketsCommandHandler> _logger;

    public CreateRewindMarketsCommandHandler(IMediator mediator, ILogger<CreateRewindMarketsCommandHandler> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(CreateRewindMarketsCommand request, CancellationToken cancellationToken)
    {
        var markets = await _mediator.Send(new RetrieveMarketsByModifiedBlockQuery(request.RewindHeight));
        var staleCount = markets.Count();

        _logger.LogDebug($"Found {staleCount} stale markets.");

        int refreshFailureCount = 0;

        var marketChunks = markets.Chunk(10);

        foreach (var chunk in marketChunks)
        {
            await Task.WhenAll(chunk.Select(async market =>
            {
                bool marketRefreshed = false;

                // Note: Currently, there is nothing that changes for staking markets so there is nothing to rewind.
                // Only update non-staking markets and check the owner. In the future if staking markets have updates to
                // apply (they shouldn't), just make refreshOwner: !market.IsStakingMarket and only refresh the owner in standard markets.
                if (!market.IsStakingMarket)
                {
                    var marketId = await _mediator.Send(new MakeMarketCommand(market,
                                                                              request.RewindHeight,
                                                                              refreshPendingOwner: true,
                                                                              refreshOwner: true));

                    marketRefreshed = marketId > 0;
                }

                if (!marketRefreshed) refreshFailureCount++;
            }));
        }

        _logger.LogDebug($"Refreshed {staleCount - refreshFailureCount} markets.");

        if (refreshFailureCount > 0) _logger.LogError($"Failed to refresh {refreshFailureCount} stale markets.");

        return refreshFailureCount == 0;
    }
}