using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Governances;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Governances;
using Opdex.Platform.Application.Abstractions.Queries.Governances;
using Opdex.Platform.Application.Abstractions.Queries.Governances.Nominations;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.MiningPools;
using Opdex.Platform.Domain.Models.Governances;
using Opdex.Platform.Domain.Models.TransactionLogs.Governances;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Governances;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Governances
{
    public class ProcessNominationLogCommandHandler : ProcessLogCommandHandler, IRequestHandler<ProcessNominationLogCommand, bool>
    {
        private readonly ILogger<ProcessNominationLogCommandHandler> _logger;

        public ProcessNominationLogCommandHandler(IMediator mediator, ILogger<ProcessNominationLogCommandHandler> logger) : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessNominationLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var persisted = await MakeTransactionLog(request.Log);
                if (!persisted)
                {
                    return false;
                }

                // Get all DB Current Nominations
                var currentNominations = await _mediator.Send(new RetrieveActiveMiningGovernanceNominationsQuery(), CancellationToken.None);
                var governance = await _mediator.Send(new RetrieveMiningGovernanceByAddressQuery(request.Log.Contract));

                // Skip updates if records are newer than the log
                var currentNominationsList = currentNominations.ToList();
                if (currentNominationsList.Any(n => n.ModifiedBlock > request.BlockHeight))
                {
                    return true;
                }

                // Get all latest Nominations from the governance contract
                var latestNominationDtos = await _mediator.Send(new CallCirrusGetMiningGovernanceSummaryNominationsQuery(request.Log.Contract, request.BlockHeight));
                var latestNominations = await Task.WhenAll(latestNominationDtos.Select(nominationDto => BuildLatestNomination(governance.Id, nominationDto,
                                                                                                                              request.BlockHeight)));

                // Update all current nominations statuses
                foreach (var currentNomination in currentNominationsList)
                {
                    var matchingLatest = latestNominations.SingleOrDefault(latest => latest.LiquidityPoolId == currentNomination.LiquidityPoolId);

                    // Current is not in latest, disable it.
                    if (matchingLatest == null)
                    {
                        currentNomination.SetStatus(false, request.BlockHeight);
                    }
                    else
                    {
                        currentNomination.SetWeight(matchingLatest.Weight, request.BlockHeight);
                    }

                    await _mediator.Send(new MakeMiningGovernanceNominationCommand(currentNomination));
                }

                // Handle latest nominations updates/inserts that weren't already current nominations
                // LatestNominations are new MiningGovernanceNomination instances. For each latest nomination that
                // is not a current nomination, attempt to retrieve an existing DB record for that nomination.
                foreach (var latest in latestNominations)
                {
                    var matchingCurrent = currentNominationsList.SingleOrDefault(current => current.LiquidityPoolId == latest.LiquidityPoolId);

                    // Skip this latest nomination if its in our current list, we've already updated it.
                    if (matchingCurrent != null)
                    {
                        continue;
                    }

                    var nomination = await _mediator.Send(new RetrieveMiningGovernanceNominationByLiquidityAndMiningPoolIdQuery(governance.Id,
                                                                                                                                latest.LiquidityPoolId,
                                                                                                                                latest.MiningPoolId,
                                                                                                                                findOrThrow: false));

                    if (nomination == null)
                    {
                        nomination = latest;
                    }
                    else
                    {
                        nomination.SetWeight(latest.Weight, request.BlockHeight);
                        nomination.SetStatus(true, request.BlockHeight);
                    }

                    await _mediator.Send(new MakeMiningGovernanceNominationCommand(nomination));
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(NominationLog)}");

                return false;
            }
        }

        private async Task<MiningGovernanceNomination> BuildLatestNomination(long governanceId, MiningGovernanceNominationCirrusDto nomination, ulong blockHeight)
        {
            var liquidityPool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(nomination.StakingPool, findOrThrow: true));
            var miningPool = await _mediator.Send(new RetrieveMiningPoolByLiquidityPoolIdQuery(liquidityPool.Id, findOrThrow: true));

            return new MiningGovernanceNomination(governanceId, liquidityPool.Id, miningPool.Id, true, nomination.Weight, blockHeight);
        }
    }
}
