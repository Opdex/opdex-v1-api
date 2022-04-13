using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.MiningPools;
using Opdex.Platform.Application.Abstractions.EntryCommands.MiningPools;
using Opdex.Platform.Application.Abstractions.Queries.MiningPools;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.MiningPools;

public class CreateRewindMiningPoolsCommandHandler : IRequestHandler<CreateRewindMiningPoolsCommand, bool>
{
    private readonly IMediator _mediator;
    private readonly ILogger<CreateRewindMiningPoolsCommandHandler> _logger;

    public CreateRewindMiningPoolsCommandHandler(IMediator mediator, ILogger<CreateRewindMiningPoolsCommandHandler> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(CreateRewindMiningPoolsCommand request, CancellationToken cancellationToken)
    {
        var miningPools = await _mediator.Send(new RetrieveMiningPoolsByModifiedBlockQuery(request.RewindHeight));
        var staleCount = miningPools.Count();

        _logger.LogDebug($"Found {staleCount} stale mining pools.");

        int refreshFailureCount = 0;

        var miningPoolChunks = miningPools.Chunk(5);

        foreach (var chunk in miningPoolChunks)
        {
            await Task.WhenAll(chunk.Select(async miningPool =>
            {
                var miningPoolId = await _mediator.Send(new MakeMiningPoolCommand(miningPool,
                                                                                  request.RewindHeight,
                                                                                  refreshRewardPerBlock: true,
                                                                                  refreshRewardPerLpt: true,
                                                                                  refreshMiningPeriodEndBlock: true));

                var miningPoolRefreshed = miningPoolId > 0;

                if (!miningPoolRefreshed) refreshFailureCount++;
            }));
        }

        _logger.LogDebug($"Refreshed {staleCount - refreshFailureCount} mining pools.");

        if (refreshFailureCount > 0) _logger.LogError($"Failed to refresh {refreshFailureCount} stale mining pools.");

        return refreshFailureCount == 0;
    }
}
