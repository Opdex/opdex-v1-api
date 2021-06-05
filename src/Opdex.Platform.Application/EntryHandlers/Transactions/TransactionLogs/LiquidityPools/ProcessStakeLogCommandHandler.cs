using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Addresses;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries;
using Opdex.Platform.Application.Abstractions.Queries.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.LiquidityPools
{
    public class ProcessStakeLogCommandHandler : ProcessLogCommandHandler, IRequestHandler<ProcessStakeLogCommand, bool>
    {
        private readonly ILogger<ProcessStakeLogCommandHandler> _logger;

        public ProcessStakeLogCommandHandler(IMediator mediator, ILogger<ProcessStakeLogCommandHandler> logger) : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessStakeLogCommand request, CancellationToken cancellationToken)
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
                
                var addressBalanceQuery = new RetrieveAddressStakingByLiquidityPoolIdAndOwnerQuery(liquidityPool.Id, request.Log.Staker, findOrThrow: false);
                var stakingBalance = await _mediator.Send(addressBalanceQuery, CancellationToken.None) 
                                     ?? new AddressStaking(liquidityPool.Id, request.Log.Staker, "0", request.BlockHeight);

                var balanceIsNewer = request.BlockHeight < stakingBalance.ModifiedBlock;
                if (balanceIsNewer && stakingBalance.Id != 0)
                {
                    return false;
                }
                
                var stakedBalanceParameters = new[] {request.Log.Staker.ToSmartContractParameter(SmartContractParameterType.Address)};
                var stakeBalanceQuery = new RetrieveCirrusLocalCallSmartContractQuery(request.Log.Contract, "GetStakedBalance", stakedBalanceParameters);
                var stakedBalanceResponse = await _mediator.Send(stakeBalanceQuery, CancellationToken.None);
                var stakedBalance = stakedBalanceResponse.DeserializeValue<string>();
                
                stakingBalance.SetWeight(stakedBalance, request.BlockHeight);

                var addressStakingCommand = new MakeAddressStakingCommand(stakingBalance);
                var addressStakingId = await _mediator.Send(addressStakingCommand, CancellationToken.None);
                    
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