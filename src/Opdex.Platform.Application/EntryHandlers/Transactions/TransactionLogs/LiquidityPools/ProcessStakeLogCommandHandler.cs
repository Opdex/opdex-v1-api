using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Addresses;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Staking;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.LiquidityPools
{
    public class ProcessStakeLogCommandHandler : IRequestHandler<ProcessStakeLogCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProcessStakeLogCommandHandler> _logger;

        public ProcessStakeLogCommandHandler(IMediator mediator, ILogger<ProcessStakeLogCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessStakeLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var liquidityPool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(request.Log.Contract, findOrThrow: true));

                var stakingBalance = await _mediator.Send(new RetrieveAddressStakingByLiquidityPoolIdAndOwnerQuery(liquidityPool.Id,
                                                                                                                   request.Log.Staker,
                                                                                                                   findOrThrow: false))
                                     ?? new AddressStaking(liquidityPool.Id, request.Log.Staker, UInt256.Zero, request.BlockHeight);

                if (request.BlockHeight < stakingBalance.ModifiedBlock)
                {
                    return true;
                }

                stakingBalance.SetWeight(request.Log.StakerBalance, request.BlockHeight);

                var addressStakingId = await _mediator.Send(new MakeAddressStakingCommand(stakingBalance));

                return addressStakingId > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(StakeLog)}");

                return false;
            }
        }
    }
}
