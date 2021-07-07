using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Governances;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Governances;
using Opdex.Platform.Application.Abstractions.Queries.Governances;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Domain.Models.Governances;
using Opdex.Platform.Domain.Models.ODX;
using Opdex.Platform.Domain.Models.TransactionLogs.Governances;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Governances;
using System;
using System.Collections.Generic;
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

                var currentNominations = await _mediator.Send(new RetrieveActiveMiningGovernanceNominationsQuery(), CancellationToken.None);

                // Skip updates if records are newer than the log
                var currentNominationsList = currentNominations as MiningGovernanceNomination[] ?? currentNominations.ToArray();
                if (currentNominationsList.Any(n => n.ModifiedBlock > request.BlockHeight))
                {
                    return true;
                }

                // Get latest nominations from contract
                var latestNominationsQuery = new CallCirrusGetMiningGovernanceSummaryNominationsQuery(request.Log.Contract);
                var latestNominationDtos = await _mediator.Send(latestNominationsQuery, CancellationToken.None);
                var latestNominations = new List<MiningGovernanceNomination>();

                foreach (var nomination in latestNominationDtos)
                {
                    var liquidityPoolQuery = new RetrieveLiquidityPoolByAddressQuery(nomination.StakingToken, findOrThrow: true);
                    var liquidityPool = await _mediator.Send(liquidityPoolQuery, CancellationToken.None);

                    var miningPoolQuery = new RetrieveMiningPoolByLiquidityPoolIdQuery(liquidityPool.Id, findOrThrow: true);
                    var miningPool = await _mediator.Send(miningPoolQuery, CancellationToken.None);

                    latestNominations.Add(new MiningGovernanceNomination(liquidityPool.Id, miningPool.Id, true, nomination.Weight, request.BlockHeight));
                }

                // Disable nominations no longer qualified
                foreach (var currentNomination in currentNominationsList)
                {
                    if (latestNominations.Any(n => n.LiquidityPoolId == currentNomination.LiquidityPoolId))
                    {
                        continue;
                    }

                    // Disable the nomination
                    currentNomination.SetNominationStatus(false, request.BlockHeight);
                    var nominationCommand = new MakeMiningGovernanceNominationCommand(currentNomination);
                    var nominationId = await _mediator.Send(nominationCommand, CancellationToken.None);
                }

                // Enable nominations not already enabled
                foreach (var nomination in latestNominations)
                {
                    if (currentNominationsList.Any(n => n.LiquidityPoolId == nomination.LiquidityPoolId))
                    {
                        continue;
                    }

                    // enable the nomination
                    var nominationCommand = new MakeMiningGovernanceNominationCommand(nomination);
                    var nominationId = await _mediator.Send(nominationCommand, CancellationToken.None);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(NominationLog)}");

                return false;
            }
        }
    }
}
