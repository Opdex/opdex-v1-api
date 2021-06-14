using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Snapshots;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Tokens.Snapshots
{
    public class SelectTokenSnapshotWithFilterQueryHandler : IRequestHandler<SelectTokenSnapshotWithFilterQuery, TokenSnapshot>
    {
        private static readonly string SqlQuery =
            @$"Select 
                {nameof(TokenSnapshotEntity.Id)},
                {nameof(TokenSnapshotEntity.TokenId)},
                {nameof(TokenSnapshotEntity.Price)},
                {nameof(TokenSnapshotEntity.StartDate)},
                {nameof(TokenSnapshotEntity.EndDate)},
                {nameof(TokenSnapshotEntity.SnapshotTypeId)}
            FROM token_snapshot
            WHERE {nameof(TokenSnapshotEntity.TokenId)} = @{nameof(SqlParams.TokenId)}
                AND 
                    (
                        @{nameof(SqlParams.Date)} BETWEEN 
                            {nameof(TokenSnapshotEntity.StartDate)} AND {nameof(TokenSnapshotEntity.EndDate)}
                        OR 
                        @{nameof(SqlParams.Date)} > {nameof(TokenSnapshotEntity.EndDate)}
                    )
                AND {nameof(TokenSnapshotEntity.SnapshotTypeId)} = @{nameof(SqlParams.SnapshotTypeId)}
            ORDER BY {nameof(TokenSnapshotEntity.EndDate)} DESC
            LIMIT 1;";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectTokenSnapshotWithFilterQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<TokenSnapshot> Handle(SelectTokenSnapshotWithFilterQuery request, CancellationToken cancellationToken)
        {
            var queryParams = new SqlParams(request.TokenId, request.MarketId, request.DateTime, (int)request.SnapshotType);
            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationToken);

            var result = await _context.ExecuteFindAsync<TokenSnapshotEntity>(query);

            return result == null
                ? new TokenSnapshot(request.TokenId, request.MarketId, request.SnapshotType, request.DateTime)
                : _mapper.Map<TokenSnapshot>(result);
        }

        private sealed class SqlParams
        {
            internal SqlParams(long tokenId, long marketId, DateTime date, int snapshotType)
            {
                TokenId = tokenId;
                MarketId = marketId;
                Date = date;
                SnapshotTypeId = snapshotType;
            }

            public long TokenId { get; }
            public long MarketId { get; }
            public DateTime Date { get; }
            public int SnapshotTypeId { get; }
        }
    }
}