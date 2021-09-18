using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Addresses;
using Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Staking;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Staking;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Balances;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Addresses.Staking
{
    public class CreateRewindStakingPositionsCommandHandler : IRequestHandler<CreateRewindStakingPositionsCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CreateRewindStakingPositionsCommandHandler> _logger;

        public CreateRewindStakingPositionsCommandHandler(IMediator mediator, ILogger<CreateRewindStakingPositionsCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(CreateRewindStakingPositionsCommand request, CancellationToken cancellationToken)
        {
            var staleStakingPositions = await _mediator.Send(new RetrieveStakingPositionsByModifiedBlockQuery(request.RewindHeight), cancellationToken);
            var staleCount = staleStakingPositions.Count();

            _logger.LogDebug($"Found {staleCount} stale staking positions.");

            var stakingPositionsByPool = staleStakingPositions.GroupBy(position => position.LiquidityPoolId);

            int refreshFailureCount = 0;

            foreach (var group in stakingPositionsByPool)
            {
                var pool = await _mediator.Send(new RetrieveLiquidityPoolByIdQuery(group.Key));

                var stakingPositionChunks = group.Chunk(10);

                foreach (var chunk in stakingPositionChunks)
                {
                    var callResults = await Task.WhenAll(chunk.Select(stakingPosition => RefreshPosition(pool.Address, stakingPosition, request.RewindHeight)));
                    refreshFailureCount += callResults.Count(succeeded => !succeeded);
                }
            }

            _logger.LogDebug($"Refreshed {staleCount - refreshFailureCount} staking positions.");

            if (refreshFailureCount > 0) _logger.LogWarning($"Failed to refresh {refreshFailureCount} stale staking positions.");

            return refreshFailureCount == 0;

            async Task<bool> RefreshPosition(Address stakingPool, AddressStaking stakingPosition, ulong rewindHeight)
            {
                var balance = await _mediator.Send(new CallCirrusGetStakingWeightForAddressQuery(stakingPool, stakingPosition.Owner, request.RewindHeight));
                stakingPosition.SetWeight(balance, request.RewindHeight);
                var id = await _mediator.Send(new MakeAddressStakingCommand(stakingPosition));
                return id != 0;
            }
        }
    }
}
