using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Addresses;
using Opdex.Platform.Application.Abstractions.Commands.MiningPools;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MiningPools;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Mining;
using Opdex.Platform.Application.Abstractions.Queries.MiningPools;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningPools;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.MiningPools;

public class ProcessMineLogCommandHandler : IRequestHandler<ProcessMineLogCommand, bool>
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProcessMineLogCommandHandler> _logger;

    public ProcessMineLogCommandHandler(IMediator mediator, ILogger<ProcessMineLogCommandHandler> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(ProcessMineLogCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var miningPool = await _mediator.Send(new RetrieveMiningPoolByAddressQuery(request.Log.Contract, findOrThrow: false));
            if (miningPool == null) return false;

            var miningBalance = await _mediator.Send(new RetrieveAddressMiningByMiningPoolIdAndOwnerQuery(miningPool.Id, request.Log.Miner, findOrThrow: false))
                                ?? new AddressMining(miningPool.Id, request.Log.Miner, UInt256.Zero, request.BlockHeight);

            // Update the mining position if this log is equal or later than the modified block
            if (request.BlockHeight >= miningBalance.ModifiedBlock)
            {
                miningBalance.SetBalance(request.Log.MinerBalance, request.BlockHeight);

                var miningPositionId = await _mediator.Send(new MakeAddressMiningCommand(miningBalance));

                if (miningPositionId == 0)
                {
                    // Don't exit here, we will want to attempt to update the mining pool below
                    _logger.LogWarning($"Unexpected error updating mining position for {request.Log.Miner}");
                }
            }

            return await _mediator.Send(new MakeMiningPoolCommand(miningPool, request.BlockHeight, refreshRewardPerLpt: true)) > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failure processing {nameof(MineLog)}");

            return false;
        }
    }
}