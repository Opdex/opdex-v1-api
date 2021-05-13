using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pools;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Pools
{
    public class SelectLiquidityPoolByTokenIdAndMarketAddressQueryHandler : IRequestHandler<SelectLiquidityPoolByTokenIdAndMarketAddressQuery, Token>
    {
        private static readonly string SqlQuery = 
            @$"SELECT 
                pl.{nameof(LiquidityPoolEntity.Id)},
                pl.{nameof(LiquidityPoolEntity.Address)},
                pl.{nameof(LiquidityPoolEntity.TokenId)},
                pl.{nameof(LiquidityPoolEntity.MarketId)}
            FROM pool_liquidity pl
            JOIN market m ON m.{nameof(MarketEntity.Id)} = pl.{nameof(LiquidityPoolEntity.MarketId)}
            WHERE pl.{nameof(LiquidityPoolEntity.TokenId)} = @{nameof(SqlParams.TokenId)}
                AND m.{nameof(MarketEntity.Address)} = @{nameof(SqlParams.MarketAddress)}
            LIMIT 1;";
                        
        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        
        public SelectLiquidityPoolByTokenIdAndMarketAddressQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        public async Task<Token> Handle(SelectLiquidityPoolByTokenIdAndMarketAddressQuery request, CancellationToken cancellationToken)
        {
            var queryParams = new SqlParams(request.TokenId, request.MarketAddress);
            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationToken);
            
            var result = await _context.ExecuteFindAsync<TokenEntity>(query);

            if (result == null)
            {
                throw new NotFoundException($"Could not find liquidity pool with tokenId {request.TokenId} and market {request.MarketAddress}");
            }

            return _mapper.Map<Token>(result);
        }

        private sealed class SqlParams
        {
            internal SqlParams(long tokenId, string marketAddress)
            {
                TokenId = tokenId;
                MarketAddress = marketAddress;
            }
            
            public long TokenId { get; }
            public string MarketAddress { get; }
        }
    }
}