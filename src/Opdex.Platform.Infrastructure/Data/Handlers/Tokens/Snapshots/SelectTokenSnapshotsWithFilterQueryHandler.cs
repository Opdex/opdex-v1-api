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
    public class SelectTokenSnapshotsWithFilterQueryHandler : IRequestHandler<SelectTokenSnapshotsWithFilterQuery, IEnumerable<TokenSnapshot>>
    {
        private static readonly string SqlQuery =
            @$"SELECT
                {nameof(TokenSnapshotEntity.Id)},
                {nameof(TokenSnapshotEntity.TokenId)},
                {nameof(TokenSnapshotEntity.MarketId)},
                {nameof(TokenSnapshotEntity.Details)},
                {nameof(TokenSnapshotEntity.StartDate)},
                {nameof(TokenSnapshotEntity.EndDate)},
                {nameof(TokenSnapshotEntity.SnapshotTypeId)},
                {nameof(TokenSnapshotEntity.ModifiedDate)}
            FROM token_snapshot
            WHERE
                {nameof(TokenSnapshotEntity.TokenId)} = @{nameof(SqlParams.TokenId)} AND
                {nameof(TokenSnapshotEntity.MarketId)} = @{nameof(SqlParams.MarketId)} AND
                {nameof(TokenSnapshotEntity.EndDate)} BETWEEN @{nameof(SqlParams.StartDate)} AND @{nameof(TokenSnapshotEntity.EndDate)}
                AND {nameof(TokenSnapshotEntity.SnapshotTypeId)} = @{nameof(SqlParams.SnapshotTypeId)}
            ORDER BY {nameof(TokenSnapshotEntity.EndDate)} DESC
            LIMIT 750;"; // Limit 750, there's about 730 hours in a month (hourly snapshots)

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectTokenSnapshotsWithFilterQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<TokenSnapshot>> Handle(SelectTokenSnapshotsWithFilterQuery request, CancellationToken cancellationToken)
        {
            var queryParams = new SqlParams(request.TokenId, request.MarketId, request.StartDate, request.EndDate, (int)request.SnapshotType);

            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationToken);

            var result = await _context.ExecuteQueryAsync<TokenSnapshotEntity>(query);

            return _mapper.Map<IEnumerable<TokenSnapshot>>(result);
        }

        private sealed class SqlParams
        {
            internal SqlParams(ulong tokenId, ulong marketId, DateTime startDate, DateTime endDate, int snapshotTypeId)
            {
                TokenId = tokenId;
                MarketId = marketId;
                StartDate = startDate;
                EndDate = endDate;
                SnapshotTypeId = snapshotTypeId;
            }

            public ulong TokenId { get; }
            public ulong MarketId { get; }
            public DateTime StartDate { get; }
            public DateTime EndDate { get; }
            public int SnapshotTypeId { get; }
        }
    }
}
