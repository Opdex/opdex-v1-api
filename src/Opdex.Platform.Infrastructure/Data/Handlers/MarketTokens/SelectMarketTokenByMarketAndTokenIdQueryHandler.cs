using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.MarketTokens;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.MarketTokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MarketTokens;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.MarketTokens
{
    public class SelectMarketTokenByMarketAndTokenIdQueryHandler : IRequestHandler<SelectMarketTokenByMarketAndTokenIdQuery, MarketToken>
    {
        private static readonly string SqlQuery =
            @$"Select
                {nameof(MarketTokenEntity.Id)},
                {nameof(MarketTokenEntity.MarketId)},
                {nameof(MarketTokenEntity.TokenId)},
                {nameof(MarketTokenEntity.CreatedBlock)},
                {nameof(MarketTokenEntity.ModifiedBlock)}
            FROM market_token
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
