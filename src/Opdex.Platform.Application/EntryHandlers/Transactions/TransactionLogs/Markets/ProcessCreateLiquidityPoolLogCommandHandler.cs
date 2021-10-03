using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.LiquidityPools;
using Opdex.Platform.Application.Abstractions.EntryCommands.MiningPools;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Markets;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Domain.Models.TransactionLogs.Markets;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Markets
{
    public class ProcessCreateLiquidityPoolLogCommandHandler : ProcessLogCommandHandler, IRequestHandler<ProcessCreateLiquidityPoolLogCommand, bool>
    {
        private readonly ILogger<ProcessCreateLiquidityPoolLogCommandHandler> _logger;

        public ProcessCreateLiquidityPoolLogCommandHandler(IMediator mediator, ILogger<ProcessCreateLiquidityPoolLogCommandHandler> logger) : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessCreateLiquidityPoolLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var persisted = await MakeTransactionLog(request.Log);
                if (!persisted)
                {
                    return false;
                }

                var market = await _mediator.Send(new RetrieveMarketByAddressQuery(request.Log.Contract, findOrThrow: true));
                var srcTokenId = await _mediator.Send(new CreateTokenCommand(request.Log.Token, request.BlockHeight));
                var lpTokenId = await _mediator.Send(new CreateTokenCommand(request.Log.Pool, request.BlockHeight));

                var liquidityPool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(request.Log.Pool, findOrThrow: false)) ??
                                    new LiquidityPool(request.Log.Pool, srcTokenId, lpTokenId, market.Id, request.BlockHeight);

                ulong liquidityPoolId = liquidityPool.Id;
                var isNewLiquidityPool = liquidityPoolId == 0;
                if (isNewLiquidityPool)
                {
                    liquidityPoolId = await _mediator.Send(new MakeLiquidityPoolCommand(liquidityPool));
                }

                // If it's the staking market, a new liquidity pool, and the pool src token isn't the markets staking token
                if (market.IsStakingMarket && isNewLiquidityPool && srcTokenId != market.StakingTokenId)
                {
                    await _mediator.Send(new CreateMiningPoolCommand(liquidityPool.Address, liquidityPoolId, request.BlockHeight));
                }

                return liquidityPoolId > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(CreateLiquidityPoolLog)}");

                return false;
            }
        }
    }
}
