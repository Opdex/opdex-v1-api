using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Markets;
using Opdex.Platform.Application.Abstractions.EntryCommands.Markets.Permissions;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Markets.Permissions;
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

            var permissionsByMarket = marketPermissions.GroupBy(cert => cert.MarketId);

            foreach (var marketGroup in permissionsByMarket)
            {
                var market = await _mediator.Send(new RetrieveMarketByIdQuery(marketGroup.Key, findOrThrow: false));

                if (market == null)
                {
                    refreshFailureCount += marketGroup.Count();
                    _logger.LogError($"Cannot find market with id {marketGroup.Key}.");
                    continue;
                }

                var marketPermissionChunks = marketGroup.Chunk(10);

                foreach (var chunk in marketPermissionChunks)
                {
                    await Task.WhenAll(chunk.Select(async marketPermission =>
                    {
                        // Todo: This should have IncludeBlame however to retrieve this value, we have to search transaction receipts and logs.
                        var summary = await _mediator.Send(new RetrieveMarketContractPermissionSummaryQuery(market.Address,
                                                                                                            marketPermission.User,
                                                                                                            marketPermission.Permission,
                                                                                                            request.RewindHeight,
                                                                                                            includeAuthorization: true));

                        marketPermission.Update(summary);

                        var marketPermissionId = await _mediator.Send(new MakeMarketPermissionCommand(marketPermission));

                        var marketPermissionRefreshed = marketPermissionId > 0;

                        if (!marketPermissionRefreshed) refreshFailureCount++;
                    }));
                }
            }

            _logger.LogDebug($"Refreshed {staleCount - refreshFailureCount} market permissions.");

            if (refreshFailureCount > 0) _logger.LogError($"Failed to refresh {refreshFailureCount} stale market permissions.");

            return refreshFailureCount == 0;
        }
    }
}
