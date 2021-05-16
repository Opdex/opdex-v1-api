using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Addresses;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs;
using Opdex.Platform.Application.Abstractions.Queries.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs
{
    public class ProcessStartStakingLogCommandHandler : IRequestHandler<ProcessStartStakingLogCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProcessStartStakingLogCommandHandler> _logger;

        public ProcessStartStakingLogCommandHandler(IMediator mediator, ILogger<ProcessStartStakingLogCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessStartStakingLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var pool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(request.Log.Contract), CancellationToken.None);
                
                var stakingBalance = await _mediator.Send(new RetrieveAddressStakingByLiquidityPoolIdAndOwnerQuery(pool.Id, request.Log.Staker), CancellationToken.None) 
                                     ?? new AddressStaking(pool.Id, request.Log.Staker, request.Log.Amount, request.BlockHeight, request.BlockHeight);

                if (request.BlockHeight <= stakingBalance.ModifiedBlock)
                {
                    return true;
                }
                
                if (stakingBalance.Id != 0)
                {
                    stakingBalance.SetWeight(request.Log, request.BlockHeight);
                }
                
                var stakingBalanceId = await _mediator.Send(new MakeAddressStakingCommand(stakingBalance), CancellationToken.None);
                    
                return stakingBalanceId > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(StartStakingLog)}");
               
                return false;
            }
        }
    }
}