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
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Domain.Models.TransactionLogs.Markets;
using System.Linq;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Markets;

public class ProcessCreateLiquidityPoolLogCommandHandler : IRequestHandler<ProcessCreateLiquidityPoolLogCommand, bool>
{
    private readonly OpdexConfiguration _config;
    private readonly IMediator _mediator;
    private readonly ILogger<ProcessCreateLiquidityPoolLogCommandHandler> _logger;

    public ProcessCreateLiquidityPoolLogCommandHandler(OpdexConfiguration config, IMediator mediator,
                                                       ILogger<ProcessCreateLiquidityPoolLogCommandHandler> logger)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(ProcessCreateLiquidityPoolLogCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var market = await _mediator.Send(new RetrieveMarketByAddressQuery(request.Log.Contract, findOrThrow: false));
            if (market == null) return false;

            var srcAttributes = new[] { TokenAttributeType.NonProvisional };
            var srcTokenId = await _mediator.Send(new CreateTokenCommand(request.Log.Token, srcAttributes, request.BlockHeight), CancellationToken.None);

            // Validate the SRC token in the pool is not an OLPT token, Opdex does not support these liquidity pools
            var srcToken = await _mediator.Send(new RetrieveTokenByIdQuery(srcTokenId));

            var tokenAttributes = await _mediator.Send(new RetrieveTokenAttributesByTokenIdQuery(srcToken.Id), CancellationToken.None);
            var isLpt = tokenAttributes.Select(attr => attr.AttributeType).Contains(TokenAttributeType.Provisional);

            if (isLpt) return false;

            var lpAttributes = new[] { TokenAttributeType.Provisional };
            var lpTokenId = await _mediator.Send(new CreateTokenCommand(request.Log.Pool, lpAttributes, request.BlockHeight), CancellationToken.None);

            var networkPrefix = _config.Network.NetworkTokenPrefix();
            var name = $"{srcToken.Symbol}-{networkPrefix}CRS";

            var liquidityPool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(request.Log.Pool, findOrThrow: false)) ??
                                new LiquidityPool(request.Log.Pool, name, srcTokenId, lpTokenId, market.Id, request.BlockHeight);

            if (liquidityPool.Id > 0) return true;

            var liquidityPoolId = await _mediator.Send(new MakeLiquidityPoolCommand(liquidityPool));

            // If it's the staking market, a new liquidity pool, and the pool src token isn't the markets staking token
            if (market.IsStakingMarket && srcTokenId != market.StakingTokenId)
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
