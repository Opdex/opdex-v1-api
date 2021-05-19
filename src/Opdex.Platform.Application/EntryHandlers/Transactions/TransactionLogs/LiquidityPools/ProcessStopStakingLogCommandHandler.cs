using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Addresses;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.LiquidityPools
{
    public class ProcessStopStakingLogCommandHandler : IRequestHandler<ProcessStopStakingLogCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProcessStopStakingLogCommandHandler> _logger;

        public ProcessStopStakingLogCommandHandler(IMediator mediator, ILogger<ProcessStopStakingLogCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessStopStakingLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var poolQuery = new RetrieveLiquidityPoolByAddressQuery(request.Log.Contract, findOrThrow: true);
                
                var pool = await _mediator.Send(poolQuery, CancellationToken.None);
                
                var addressBalanceQuery = new RetrieveAddressStakingByLiquidityPoolIdAndOwnerQuery(pool.Id, request.Log.Staker, findOrThrow: false);

                const string resetStakingBalance = "0";
                
                var stakingBalance = await _mediator.Send(addressBalanceQuery, CancellationToken.None) 
                                     ?? new AddressStaking(pool.Id, request.Log.Staker, resetStakingBalance, request.BlockHeight);

                // potentially stale log, ignore
                if (request.BlockHeight <= stakingBalance.ModifiedBlock)
                {
                    return true;
                }
                
                // Update if if it's not a new record, else it's already been created and is current
                if (stakingBalance.Id != 0)
                {
                    stakingBalance.ResetWeight(request.Log, request.BlockHeight);
                }
                
                var stakingBalanceId = await _mediator.Send(new MakeAddressStakingCommand(stakingBalance), CancellationToken.None);
                    
                return stakingBalanceId > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(StopStakingLog)}");
               
                return false;
            }
        }
    }
}