using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.MiningGovernances;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MiningGovernances;
using Opdex.Platform.Application.Abstractions.Queries.MiningGovernances;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningGovernances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.MiningGovernances;

public class ProcessRewardMiningPoolLogCommandHandler : IRequestHandler<ProcessRewardMiningPoolLogCommand, bool>
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProcessRewardMiningPoolLogCommandHandler> _logger;

    public ProcessRewardMiningPoolLogCommandHandler(IMediator mediator, ILogger<ProcessRewardMiningPoolLogCommandHandler> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(ProcessRewardMiningPoolLogCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var miningGovernance = await _mediator.Send(new RetrieveMiningGovernanceByAddressQuery(request.Log.Contract, findOrThrow: false));
            if (miningGovernance == null) return false;

            if (request.BlockHeight < miningGovernance.ModifiedBlock)
            {
                return true;
            }

            var updateId = await _mediator.Send(new MakeMiningGovernanceCommand(miningGovernance, request.BlockHeight,
                                                                                refreshMiningPoolReward: true,
                                                                                refreshNominationPeriodEnd: true,
                                                                                refreshMiningPoolsFunded: true));

            return updateId > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failure processing {nameof(RewardMiningPoolLog)}");

            return false;
        }
    }
}