using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Deployers;
using Opdex.Platform.Application.Abstractions.EntryCommands.Deployers;
using Opdex.Platform.Application.Abstractions.Queries.Deployers;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Deployers
{
    public class CreateRewindDeployersCommandHandler : IRequestHandler<CreateRewindDeployersCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CreateRewindDeployersCommandHandler> _logger;

        public CreateRewindDeployersCommandHandler(IMediator mediator, ILogger<CreateRewindDeployersCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(CreateRewindDeployersCommand request, CancellationToken cancellationToken)
        {
            var deployers = await _mediator.Send(new RetrieveDeployersByModifiedBlockQuery(request.RewindHeight));
            var staleCount = deployers.Count();

            _logger.LogDebug($"Found {staleCount} stale deployers.");

            // Split the deployers into chunks of 10
            var deployersChunks = deployers.Chunk(10);

            int refreshFailureCount = 0;

            foreach (var chunk in deployersChunks)
            {
                // Each chunk runs in parallel
                var callResults = await Task.WhenAll(chunk.Select(async deployer =>
                {
                    var id = await _mediator.Send(new MakeDeployerCommand(deployer, request.RewindHeight, refreshPendingOwner: true, refreshOwner: true));
                    return id != 0;
                }));
                refreshFailureCount += callResults.Count(succeeded => !succeeded);
            }

            _logger.LogDebug($"Refreshed {staleCount - refreshFailureCount} deployers.");

            if (refreshFailureCount > 0) _logger.LogError($"Failed to refresh {refreshFailureCount} stale deployers.");

            return refreshFailureCount == 0;
        }
    }
}
