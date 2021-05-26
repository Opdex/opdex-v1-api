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
    public class ProcessStopStakingLogCommandHandler : ProcessLogCommandHandler, IRequestHandler<ProcessStopStakingLogCommand, bool>
    {
        private readonly ILogger<ProcessStopStakingLogCommandHandler> _logger;

        public ProcessStopStakingLogCommandHandler(IMediator mediator, ILogger<ProcessStopStakingLogCommandHandler> logger) : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessStopStakingLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var persisted = await MakeTransactionLog(request.Log);
                if (!persisted)
                {
                    return false;
                }
                
                var liquidityPoolQuery = new RetrieveLiquidityPoolByAddressQuery(request.Log.Contract, findOrThrow: true);
                var liquidityPool = await _mediator.Send(liquidityPoolQuery, CancellationToken.None);
                
                var addressBalanceQuery = new RetrieveAddressStakingByLiquidityPoolIdAndOwnerQuery(liquidityPool.Id, request.Log.Staker, findOrThrow: true);
                var stakingBalance = await _mediator.Send(addressBalanceQuery, CancellationToken.None);

                if (request.BlockHeight < stakingBalance.ModifiedBlock)
                {
                    return true;
                }
                
                stakingBalance.ResetWeight(request.Log, request.BlockHeight);

                var addressStakingCommand = new MakeAddressStakingCommand(stakingBalance);
                var addressStakingId = await _mediator.Send(addressStakingCommand, CancellationToken.None);
                    
                return addressStakingId > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(StopStakingLog)}");
               
                return false;
            }
        }
    }
}