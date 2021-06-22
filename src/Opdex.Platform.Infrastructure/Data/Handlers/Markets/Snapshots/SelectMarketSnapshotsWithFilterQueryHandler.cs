using MediatR;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets.Snapshots;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Markets.Snapshots
{
    public class SelectMarketSnapshotsWithFilterQueryHandler
        : IRequestHandler<SelectMarketSnapshotsWithFilterQuery, IEnumerable<MarketSnapshot>>
    {
        private static readonly string SqlQuery =
            @$"Select
                {nameof(MarketSnapshotEntity.Id)},
                {nameof(MarketSnapshotEntity.MarketId)},
                {nameof(MarketSnapshotEntity.Details)},
                {nameof(MarketSnapshotEntity.SnapshotTypeId)},
                {nameof(MarketSnapshotEntity.StartDate)},
                {nameof(MarketSnapshotEntity.EndDate)}
            FROM market_snapshot
           WHERE {nameof(MarketSnapshotEntity.MarketId)} = @{nameof(SqlParams.MarketId)}
                AND @{nameof(SqlParams.Start)} <= {nameof(MarketSnapshotEntity.StartDate)}
                AND @{nameof(SqlParams.End)} >= {nameof(MarketSnapshotEntity.EndDate)}
                AND {nameof(MarketSnapshotEntity.SnapshotTypeId)} = @{nameof(SqlParams.SnapshotTypeId)}
            ORDER BY {nameof(MarketSnapshotEntity.EndDate)} DESC
            LIMIT 750;"; // Limit 750, there's about 730 hours in a month (hourly snapshots)

        private readonly IDbContext _context;
        private readonly AutoMapper.IMapper _mapper;

        public SelectMarketSnapshotsWithFilterQueryHandler(IDbContext context, AutoMapper.IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<MarketSnapshot>> Handle(SelectMarketSnapshotsWithFilterQuery request, CancellationToken cancellationToken)
        {
            var queryParams = new SqlParams(request.MarketId, request.StartDate, request.EndDate, (int)request.SnapshotType);

            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationToken);

            var result = await _context.ExecuteQueryAsync<MarketSnapshotEntity>(query);

            return _mapper.Map<IEnumerable<MarketSnapshot>>(result);
        }

        private sealed class SqlParams
        {
            internal SqlParams(long marketId, DateTime start, DateTime end, int snapshotTypeId)
            {
                MarketId = marketId;
                Start = start;
                End = end;
                SnapshotTypeId = snapshotTypeId;
            }

            public long MarketId { get; }
            public DateTime Start { get; }
            public DateTime End { get; }
            public int SnapshotTypeId { get; }
        }
    }
}
