using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Pools;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Pools
{
    public class PersistLiquidityPoolCommandHandler : IRequestHandler<PersistLiquidityPoolCommand, long>
    {
        // Todo: Consider schema changes around MarketId and the ability to update markets
        private static readonly string SqlCommand =
            $@"INSERT INTO pool_liquidity (
                {nameof(LiquidityPoolEntity.Address)},
                {nameof(LiquidityPoolEntity.SrcTokenId)},
                {nameof(LiquidityPoolEntity.LpTokenId)},
                {nameof(LiquidityPoolEntity.MarketId)},
                {nameof(LiquidityPoolEntity.CreatedBlock)},
                {nameof(LiquidityPoolEntity.ModifiedBlock)}
              ) VALUES (
                @{nameof(LiquidityPoolEntity.Address)},
                @{nameof(LiquidityPoolEntity.SrcTokenId)},
                @{nameof(LiquidityPoolEntity.LpTokenId)},
                @{nameof(LiquidityPoolEntity.MarketId)},
                @{nameof(LiquidityPoolEntity.CreatedBlock)},
                @{nameof(LiquidityPoolEntity.ModifiedBlock)}
              );
              SELECT LAST_INSERT_ID();";

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

        public async Task<long> Handle(PersistLiquidityPoolCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var poolEntity = _mapper.Map<LiquidityPoolEntity>(request.Pool);

                var command = DatabaseQuery.Create(SqlCommand, poolEntity, cancellationToken);

                return await _context.ExecuteScalarAsync<long>(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unable to persist {request.Pool}");
                return 0;
            }
        }
    }
}