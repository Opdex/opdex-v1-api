using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.LiquidityPools;

public class ProcessCollectStakingRewardsLogCommandHandler : IRequestHandler<ProcessCollectStakingRewardsLogCommand, bool>
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProcessCollectStakingRewardsLogCommandHandler> _logger;

    public ProcessCollectStakingRewardsLogCommandHandler(IMediator mediator, ILogger<ProcessCollectStakingRewardsLogCommandHandler> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(ProcessCollectStakingRewardsLogCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var liquidityPool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(request.Log.Contract, findOrThrow: false));

            return liquidityPool != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failure processing {nameof(CollectStakingRewardsLog)}");

            return false;
        }
    }
}