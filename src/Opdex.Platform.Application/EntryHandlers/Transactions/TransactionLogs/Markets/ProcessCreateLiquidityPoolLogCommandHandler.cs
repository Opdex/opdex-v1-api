using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Commands.MiningPools;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Markets;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Domain.Models.MiningPools;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Domain.Models.TransactionLogs.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;

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

                var market = await _mediator.Send(new RetrieveMarketByAddressQuery(request.Log.Contract), CancellationToken.None);

                var srcTokenId = await MakeToken(request.Log.Token, request.BlockHeight);

                var lpTokenId = await MakeToken(request.Log.Pool, request.BlockHeight, true);

                var liquidityPool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(request.Log.Pool, findOrThrow: false));
                long liquidityPoolId = 0;

                if (liquidityPool == null)
                {
                    liquidityPool = new LiquidityPool(request.Log.Pool, srcTokenId, lpTokenId, market.Id, request.BlockHeight);
                    liquidityPoolId = await _mediator.Send(new MakeLiquidityPoolCommand(liquidityPool));
                }

                // If it's the staking market, a new liquidity pool, and the pool src token isn't the markets staking token
                if (market.StakingTokenId > 0 && liquidityPool.Id == 0 && srcTokenId != market.StakingTokenId)
                {
                    var miningPoolAddress = await _mediator.Send(new CallCirrusGetMiningPoolByTokenQuery(request.Log.Pool, request.BlockHeight));

                    var miningPool = new MiningPool(liquidityPoolId, miningPoolAddress, request.BlockHeight);

                    var miningPoolId = await _mediator.Send(new MakeMiningPoolCommand(miningPool));
                }

                return liquidityPoolId > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(CreateLiquidityPoolLog)}");

                return false;
            }
        }

        private async Task<long> MakeToken(string tokenAddress, ulong blockHeight, bool isLpToken = false)
        {
            var srcToken = await _mediator.Send(new RetrieveTokenByAddressQuery(tokenAddress, findOrThrow: false));

            if (srcToken != null)
            {
                return srcToken.Id;
            }

            var summary = await _mediator.Send(new CallCirrusGetSrcTokenSummaryByAddressQuery(tokenAddress));

            srcToken = new Token(summary.Address, isLpToken, summary.Name, summary.Symbol, (int)summary.Decimals, summary.Sats,
                                 summary.TotalSupply, blockHeight);

            return await _mediator.Send(new MakeTokenCommand(srcToken));
        }
    }
}
