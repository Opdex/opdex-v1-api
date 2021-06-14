using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Pools;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Markets;
using Opdex.Platform.Application.Abstractions.Queries;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Domain.Models.Pools;
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

                var marketQuery = new RetrieveMarketByAddressQuery(request.Log.Contract, findOrThrow: true);
                var market = await _mediator.Send(marketQuery, CancellationToken.None);

                var srcTokenId = await MakeToken(request.Log.Token);
                var lpTokenId = await MakeToken(request.Log.Pool);

                var liquidityPoolQuery = new RetrieveLiquidityPoolByAddressQuery(request.Log.Pool, findOrThrow: false);
                var liquidityPool = await _mediator.Send(liquidityPoolQuery, CancellationToken.None);

                long liquidityPoolId = 0;

                if (liquidityPool == null)
                {
                    liquidityPool = new LiquidityPool(request.Log.Pool, srcTokenId, lpTokenId, market.Id, request.BlockHeight);
                    var liquidityPoolCommand = new MakeLiquidityPoolCommand(liquidityPool);
                    liquidityPoolId = await _mediator.Send(liquidityPoolCommand, CancellationToken.None);
                }

                // If it's the staking market, a new liquidity pool, and the pool src token isn't the markets staking token
                if (market.StakingTokenId > 0 && liquidityPool.Id == 0 && srcTokenId != market.StakingTokenId)
                {
                    var getMiningPoolAddressQuery = new RetrieveCirrusLocalCallSmartContractQuery(request.Log.Pool, "get_MiningPool");
                    var getMiningPoolAddressResponse = await _mediator.Send(getMiningPoolAddressQuery, CancellationToken.None);
                    var miningPoolAddress = getMiningPoolAddressResponse.DeserializeValue<string>();

                    var miningPool = new MiningPool(liquidityPoolId, miningPoolAddress, request.BlockHeight);

                    var miningPoolCommand = new MakeMiningPoolCommand(miningPool);
                    var miningPoolId = await _mediator.Send(miningPoolCommand, CancellationToken.None);
                }

                return liquidityPoolId > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(CreateLiquidityPoolLog)}");

                return false;
            }
        }

        private async Task<long> MakeToken(string tokenAddress)
        {
            var srcTokenQuery = new RetrieveTokenByAddressQuery(tokenAddress, findOrThrow: false);
            var srcToken = await _mediator.Send(srcTokenQuery);

            var srcTokenCommand = new MakeTokenCommand(tokenAddress);
            return srcToken?.Id ?? await _mediator.Send(srcTokenCommand);
        }
    }
}