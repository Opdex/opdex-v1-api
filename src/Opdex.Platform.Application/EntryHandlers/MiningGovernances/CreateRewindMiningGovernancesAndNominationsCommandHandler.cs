using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.MiningGovernances;
using Opdex.Platform.Application.Abstractions.EntryCommands.MiningGovernances;
using Opdex.Platform.Application.Abstractions.Queries.MiningGovernances;
using Opdex.Platform.Application.Abstractions.Queries.MiningGovernances.Nominations;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.MiningGovernances
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
            var miningGovernances = await _mediator.Send(new RetrieveMiningGovernancesByModifiedBlockQuery(request.RewindHeight));
            var staleCount = miningGovernances.Count();

            _logger.LogDebug($"Found {staleCount} stale mining governances.");

            int refreshFailureCount = 0;

            foreach (var miningGovernance in miningGovernances)
            {
                var miningGovernanceId = await _mediator.Send(new MakeMiningGovernanceCommand(miningGovernance,
                                                                                        request.RewindHeight,
                                                                                        refreshNominationPeriodEnd: true,
                                                                                        refreshMiningPoolsFunded: true,
                                                                                        refreshMiningPoolReward: true));

                var miningGovernanceRefreshed = miningGovernanceId != 0;

                var nominationsRefreshed = await _mediator.Send(new MakeMiningGovernanceNominationsCommand(miningGovernance, request.RewindHeight));

                if (!miningGovernanceRefreshed || !nominationsRefreshed) refreshFailureCount++;
            }

            _logger.LogDebug($"Refreshed {staleCount - refreshFailureCount} mining governances.");

            if (refreshFailureCount > 0) _logger.LogError($"Failed to refresh {refreshFailureCount} stale mining governances.");

            return refreshFailureCount == 0;
        }
    }
}
