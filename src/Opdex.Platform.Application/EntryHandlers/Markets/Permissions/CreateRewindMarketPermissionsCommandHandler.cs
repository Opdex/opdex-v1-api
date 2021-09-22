using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryCommands.Markets.Permissions;
using Opdex.Platform.Application.Abstractions.Queries.Markets.Permissions;
using Opdex.Platform.Common.Extensions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Markets.Permissions
{
    public class CreateRewindMarketPermissionsCommandHandler : IRequestHandler<CreateRewindMarketPermissionsCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CreateRewindMarketPermissionsCommandHandler> _logger;

        public CreateRewindMarketPermissionsCommandHandler(IMediator mediator, ILogger<CreateRewindMarketPermissionsCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(CreateRewindMarketPermissionsCommand request, CancellationToken cancellationToken)
        {
            var marketPermissions = await _mediator.Send(new RetrieveMarketPermissionsByModifiedBlockQuery(request.RewindHeight));
            var staleCount = marketPermissions.Count();

            _logger.LogDebug($"Found {staleCount} stale market permissions.");

            int refreshFailureCount = 0;

            var marketPermissionChunks = marketPermissions.Chunk(10);

            foreach (var chunk in marketPermissionChunks)
            {
                await Task.WhenAll(chunk.Select(async marketPermission =>
                {
                    // var marketPermissionId = await _mediator.Send(new MakeMarketPermissionCommand(marketPermission,
                    //                                                                   request.RewindHeight,
                    //                                                                   refreshRewardPerBlock: true,
                    //                                                                   refreshRewardPerLpt: true,
                    //                                                                   refreshMiningPeriodEndBlock: true));
                    //
                    // var marketPermissionRefreshed = marketPermissionId > 0;
                    //
                    // if (!marketPermissionRefreshed) refreshFailureCount++;
                }));
            }

            _logger.LogDebug($"Refreshed {staleCount - refreshFailureCount} market permissions.");

            if (refreshFailureCount > 0) _logger.LogError($"Failed to refresh {refreshFailureCount} stale market permissions.");

            return refreshFailureCount == 0;
        }
    }
}
