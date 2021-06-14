using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Snapshots;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Tokens.Snapshots
{
    public class SelectTokenSnapshotsByTokenIdAndMarketIdAndTimeQueryHandler : IRequestHandler<SelectTokenSnapshotsByTokenIdAndMarketIdAndTimeQuery, IEnumerable<TokenSnapshot>>
    {
        private static readonly string SqlQuery =
            @$"Select 
                {nameof(TokenSnapshotEntity.Id)},
                {nameof(TokenSnapshotEntity.TokenId)},
                {nameof(TokenSnapshotEntity.MarketId)},
                {nameof(TokenSnapshotEntity.Price)},
                {nameof(TokenSnapshotEntity.StartDate)},
                {nameof(TokenSnapshotEntity.EndDate)},
                {nameof(TokenSnapshotEntity.SnapshotTypeId)}
            FROM token_snapshot
            WHERE 
                {nameof(TokenSnapshotEntity.TokenId)} = @{nameof(SqlParams.TokenId)} AND 
                {nameof(TokenSnapshotEntity.MarketId)} = @{nameof(SqlParams.MarketId)} AND 
                @{nameof(SqlParams.Time)} BETWEEN {nameof(TokenSnapshotEntity.StartDate)} AND {nameof(TokenSnapshotEntity.EndDate)};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectTokenSnapshotsByTokenIdAndMarketIdAndTimeQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<TokenSnapshot>> Handle(SelectTokenSnapshotsByTokenIdAndMarketIdAndTimeQuery request, CancellationToken cancellationToken)
        {
            var queryParams = new SqlParams(request.TokenId, request.MarketId, request.Time);

            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationToken);

            var result = await _context.ExecuteQueryAsync<TokenSnapshotEntity>(query);

            return _mapper.Map<IEnumerable<TokenSnapshot>>(result);
        }

        private sealed class SqlParams
        {
            internal SqlParams(long tokenId, long marketId, DateTime time)
            {
                TokenId = tokenId;
                MarketId = marketId;
                Time = time;
            }

            public long TokenId { get; }
            public long MarketId { get; }
            public DateTime Time { get; }
        }
    }
}