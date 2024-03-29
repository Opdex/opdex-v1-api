using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.LiquidityPools;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.LiquidityPools;

public class PersistLiquidityPoolCommandHandler : IRequestHandler<PersistLiquidityPoolCommand, ulong>
{
    private static readonly string SqlCommand =
        $@"INSERT INTO pool_liquidity (
                {nameof(LiquidityPoolEntity.Address)},
                {nameof(LiquidityPoolEntity.Name)},
                {nameof(LiquidityPoolEntity.SrcTokenId)},
                {nameof(LiquidityPoolEntity.LpTokenId)},
                {nameof(LiquidityPoolEntity.MarketId)},
                {nameof(LiquidityPoolEntity.CreatedBlock)},
                {nameof(LiquidityPoolEntity.ModifiedBlock)}
              ) VALUES (
                @{nameof(LiquidityPoolEntity.Address)},
                @{nameof(LiquidityPoolEntity.Name)},
                @{nameof(LiquidityPoolEntity.SrcTokenId)},
                @{nameof(LiquidityPoolEntity.LpTokenId)},
                @{nameof(LiquidityPoolEntity.MarketId)},
                @{nameof(LiquidityPoolEntity.CreatedBlock)},
                @{nameof(LiquidityPoolEntity.ModifiedBlock)}
              );
              SELECT LAST_INSERT_ID();".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public PersistLiquidityPoolCommandHandler(IDbContext context, IMapper mapper,
                                              ILogger<PersistLiquidityPoolCommandHandler> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ulong> Handle(PersistLiquidityPoolCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var poolEntity = _mapper.Map<LiquidityPoolEntity>(request.Pool);

            var command = DatabaseQuery.Create(SqlCommand, poolEntity, cancellationToken);

            return await _context.ExecuteScalarAsync<ulong>(command);
        }
        catch (Exception ex)
        {
            using (_logger.BeginScope(new Dictionary<string, object>()
            {
                { "Contract", request.Pool.Address },
                { "BlockHeight", request.Pool.ModifiedBlock }
            }))
            {
                _logger.LogError(ex, $"Unable to persist liquidity pool.");
            }
            return 0;
        }
    }
}
