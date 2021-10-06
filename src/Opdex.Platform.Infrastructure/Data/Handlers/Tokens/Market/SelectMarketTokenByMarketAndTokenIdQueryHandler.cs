using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Market;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Tokens.Market
{
    public class SelectMarketTokenByMarketAndTokenIdQueryHandler : IRequestHandler<SelectMarketTokenByMarketAndTokenIdQuery, MarketToken>
    {
        private static readonly string SqlQuery =
            @$"SELECT
                mt.{nameof(MarketTokenEntity.Id)},
                mt.{nameof(MarketTokenEntity.MarketId)},
                mt.{nameof(MarketTokenEntity.TokenId)},
                mt.{nameof(MarketTokenEntity.CreatedBlock)},
                mt.{nameof(MarketTokenEntity.ModifiedBlock)},
                t.{nameof(TokenEntity.Address)},
                t.{nameof(TokenEntity.IsLpt)},
                t.{nameof(TokenEntity.Name)},
                t.{nameof(TokenEntity.Symbol)},
                t.{nameof(TokenEntity.Decimals)},
                t.{nameof(TokenEntity.Sats)},
                t.{nameof(TokenEntity.TotalSupply)}
            FROM market_token mt
            JOIN token t ON t.{nameof(TokenEntity.Id)} = mt.{nameof(MarketTokenEntity.TokenId)}
            WHERE {nameof(MarketTokenEntity.MarketId)} = @{nameof(SqlParams.MarketId)}
                   AND {nameof(MarketTokenEntity.TokenId)} = @{nameof(SqlParams.TokenId)}
            LIMIT 1;";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectMarketTokenByMarketAndTokenIdQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<MarketToken> Handle(SelectMarketTokenByMarketAndTokenIdQuery request, CancellationToken cancellationToken)
        {
            var query = DatabaseQuery.Create(SqlQuery, new SqlParams(request.MarketId, request.TokenId), cancellationToken);

            var result = await _context.ExecuteFindAsync<MarketTokenEntity>(query);

            if (request.FindOrThrow && result == null)
            {
                throw new NotFoundException($"{nameof(MarketToken)} not found.");
            }

            return result == null ? null : _mapper.Map<MarketToken>(result);
        }

        private sealed class SqlParams
        {
            internal SqlParams(ulong marketId, ulong tokenId)
            {
                MarketId = marketId;
                TokenId = tokenId;
            }

            public ulong MarketId { get; }
            public ulong TokenId { get; }
        }
    }
}
