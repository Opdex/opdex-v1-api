using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pools;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Pools
{
    public class SelectLiquidityPoolBySrcTokenIdAndMarketIdQueryHandler : IRequestHandler<SelectLiquidityPoolBySrcTokenIdAndMarketIdQuery, LiquidityPool>
    {
        private static readonly string SqlQuery =
            @$"SELECT 
                {nameof(LiquidityPoolEntity.Id)},
                {nameof(LiquidityPoolEntity.Address)},
                {nameof(LiquidityPoolEntity.SrcTokenId)},
                {nameof(LiquidityPoolEntity.LpTokenId)},
                {nameof(LiquidityPoolEntity.MarketId)},
                {nameof(LiquidityPoolEntity.CreatedBlock)},
                {nameof(LiquidityPoolEntity.ModifiedBlock)}
            FROM pool_liquidity
            WHERE {nameof(LiquidityPoolEntity.SrcTokenId)} = @{nameof(SqlParams.SrcTokenId)}
                AND {nameof(LiquidityPoolEntity.MarketId)} = @{nameof(SqlParams.MarketId)}
            LIMIT 1;";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectLiquidityPoolBySrcTokenIdAndMarketIdQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<LiquidityPool> Handle(SelectLiquidityPoolBySrcTokenIdAndMarketIdQuery request, CancellationToken cancellationToken)
        {
            var queryParams = new SqlParams(request.TokenId, request.MarketId);
            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationToken);

            var result = await _context.ExecuteFindAsync<LiquidityPoolEntity>(query);

            if (request.FindOrThrow && result == null)
            {
                throw new NotFoundException($"{nameof(LiquidityPool)} not found.");
            }

            return result == null ? null : _mapper.Map<LiquidityPool>(result);
        }

        private sealed class SqlParams
        {
            internal SqlParams(long srcTokenId, long marketId)
            {
                SrcTokenId = srcTokenId;
                MarketId = marketId;
            }

            public long SrcTokenId { get; }
            public long MarketId { get; }
        }
    }
}