using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.MiningPools;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MiningPools;
using Opdex.Platform.Application.Abstractions.Queries.MiningPools;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningPools;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.MiningPools;

public class ProcessCollectMiningRewardsLogCommandHandler : IRequestHandler<ProcessCollectMiningRewardsLogCommand, bool>
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProcessCollectMiningRewardsLogCommandHandler> _logger;

    public ProcessCollectMiningRewardsLogCommandHandler(IMediator mediator, ILogger<ProcessCollectMiningRewardsLogCommandHandler> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(ProcessCollectMiningRewardsLogCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var miningPool = await _mediator.Send(new RetrieveMiningPoolByAddressQuery(request.Log.Contract, findOrThrow: false));
            if (miningPool == null) return false;

            if (request.BlockHeight < miningPool.ModifiedBlock)
            {
                return true;
            }

            return await _mediator.Send(new MakeMiningPoolCommand(miningPool, request.BlockHeight, refreshRewardPerLpt: true)) > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failure processing {nameof(CollectMiningRewardsLog)}");

            return false;
        }
    }
}