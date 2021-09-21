using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Governances;
using Opdex.Platform.Application.Abstractions.EntryCommands.Governances;
using Opdex.Platform.Application.Abstractions.Queries.Governances;
using Opdex.Platform.Application.Abstractions.Queries.Governances.Nominations;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Governances
{
    public class CreateRewindMiningGovernancesAndNominationsCommandHandler
        : IRequestHandler<CreateRewindMiningGovernancesAndNominationsCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CreateRewindMiningGovernancesAndNominationsCommandHandler> _logger;

        public CreateRewindMiningGovernancesAndNominationsCommandHandler(IMediator mediator, ILogger<CreateRewindMiningGovernancesAndNominationsCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(CreateRewindMiningGovernancesAndNominationsCommand request, CancellationToken cancellationToken)
        {
            var governances = await _mediator.Send(new RetrieveMiningGovernancesByModifiedBlockQuery(request.RewindHeight));
            var staleCount = governances.Count();

            _logger.LogDebug($"Found {staleCount} stale mining governances.");

            int refreshFailureCount = 0;

            foreach (var governance in governances)
            {
                var governanceId = await _mediator.Send(new MakeMiningGovernanceCommand(governance,
                                                                                        request.RewindHeight,
                                                                                        refreshNominationPeriodEnd: true,
                                                                                        refreshMiningPoolsFunded: true,
                                                                                        refreshMiningPoolReward: true));

                var governanceRefreshed = governanceId != 0;

                var nominationsRefreshed = await _mediator.Send(new MakeGovernanceNominationsCommand(governance, request.RewindHeight));

                if (!governanceRefreshed || !nominationsRefreshed) refreshFailureCount++;
            }

            _logger.LogDebug($"Refreshed {staleCount - refreshFailureCount} mining governances.");

            if (refreshFailureCount > 0) _logger.LogError($"Failed to refresh {refreshFailureCount} stale mining governances.");

            return refreshFailureCount == 0;
        }
    }
}
