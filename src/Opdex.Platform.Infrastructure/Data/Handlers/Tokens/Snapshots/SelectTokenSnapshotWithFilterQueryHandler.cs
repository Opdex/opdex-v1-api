using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Snapshots;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Tokens.Snapshots;

public class SelectTokenSnapshotWithFilterQueryHandler : IRequestHandler<SelectTokenSnapshotWithFilterQuery, TokenSnapshot>
{
    private static readonly string SqlQuery =
        @$"SELECT
                {nameof(TokenSnapshotEntity.Id)},
                {nameof(TokenSnapshotEntity.TokenId)},
                {nameof(TokenSnapshotEntity.MarketId)},
                {nameof(TokenSnapshotEntity.Details)},
                {nameof(TokenSnapshotEntity.StartDate)},
                {nameof(TokenSnapshotEntity.EndDate)},
                {nameof(TokenSnapshotEntity.SnapshotTypeId)}
            FROM token_snapshot
            WHERE
                {nameof(TokenSnapshotEntity.TokenId)} = @{nameof(SqlParams.TokenId)} AND
                {nameof(TokenSnapshotEntity.MarketId)} = @{nameof(SqlParams.MarketId)} AND
                (
                    (@{nameof(SqlParams.DateTime)} BETWEEN {nameof(TokenSnapshotEntity.StartDate)} AND {nameof(TokenSnapshotEntity.EndDate)})
                    OR @{nameof(SqlParams.DateTime)} > {nameof(TokenSnapshotEntity.EndDate)}
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
        internal SqlParams(ulong tokenId, ulong marketId, DateTime dateTime, int snapshotType)
        {
            TokenId = tokenId;
            MarketId = marketId;
            DateTime = dateTime;
            SnapshotTypeId = snapshotType;
        }

        public ulong TokenId { get; }
        public ulong MarketId { get; }
        public DateTime DateTime { get; }
        public int SnapshotTypeId { get; }
    }
}