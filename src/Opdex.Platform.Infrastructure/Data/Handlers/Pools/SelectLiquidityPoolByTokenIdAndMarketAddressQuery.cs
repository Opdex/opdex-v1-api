using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pools;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Pools
{
    public class SelectLiquidityPoolByTokenIdAndMarketAddressQueryHandler : IRequestHandler<SelectLiquidityPoolByTokenIdAndMarketAddressQuery, LiquidityPool>
    {
        private static readonly string SqlQuery =
            @$"SELECT 
                pl.{nameof(LiquidityPoolEntity.Id)},
                pl.{nameof(LiquidityPoolEntity.Address)},
                pl.{nameof(LiquidityPoolEntity.SrcTokenId)},
                pl.{nameof(LiquidityPoolEntity.LpTokenId)},
                pl.{nameof(LiquidityPoolEntity.MarketId)},
                pl.{nameof(LiquidityPoolEntity.CreatedBlock)},
                pl.{nameof(LiquidityPoolEntity.ModifiedBlock)}
            FROM pool_liquidity pl
            JOIN market m ON m.{nameof(MarketEntity.Id)} = pl.{nameof(LiquidityPoolEntity.MarketId)}
            WHERE pl.{nameof(LiquidityPoolEntity.SrcTokenId)} = @{nameof(SqlParams.SrcTokenId)}
                AND m.{nameof(MarketEntity.Address)} = @{nameof(SqlParams.MarketAddress)}
            LIMIT 1;";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectLiquidityPoolByTokenIdAndMarketAddressQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<LiquidityPool> Handle(SelectLiquidityPoolByTokenIdAndMarketAddressQuery request, CancellationToken cancellationToken)
        {
            var queryParams = new SqlParams(request.TokenId, request.MarketAddress);
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
            internal SqlParams(long srcTokenId, string marketAddress)
            {
                SrcTokenId = srcTokenId;
                MarketAddress = marketAddress;
            }

            public long SrcTokenId { get; }
            public string MarketAddress { get; }
        }
    }
}