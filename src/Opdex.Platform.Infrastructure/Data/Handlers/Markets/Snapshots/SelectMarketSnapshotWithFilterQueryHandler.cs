using AutoMapper;
using MediatR;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets.Snapshots;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Markets;
using Opdex.Platform.Domain.Models.Markets;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Markets.Snapshots;

public class SelectMarketSnapshotWithFilterQueryHandler
    : IRequestHandler<SelectMarketSnapshotWithFilterQuery, MarketSnapshot>
{
    private static readonly string SqlCommand =
        $@"SELECT
                {nameof(MarketSnapshotEntity.Id)},
                {nameof(MarketSnapshotEntity.MarketId)},
                {nameof(MarketSnapshotEntity.Details)},
                {nameof(MarketSnapshotEntity.SnapshotTypeId)},
                {nameof(MarketSnapshotEntity.StartDate)},
                {nameof(MarketSnapshotEntity.EndDate)}
            FROM market_snapshot
            WHERE {nameof(MarketSnapshotEntity.MarketId)} = @{nameof(SqlParams.MarketId)}
                AND
                    (
                        (@{nameof(SqlParams.Date)} BETWEEN {nameof(MarketSnapshotEntity.StartDate)} AND {nameof(MarketSnapshotEntity.EndDate)})
                        OR
                        @{nameof(SqlParams.Date)} > {nameof(MarketSnapshotEntity.EndDate)}
                    )
                AND {nameof(MarketSnapshotEntity.SnapshotTypeId)} = @{nameof(SqlParams.SnapshotTypeId)}
            ORDER BY {nameof(MarketSnapshotEntity.EndDate)} DESC
            LIMIT 1;";

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectMarketSnapshotWithFilterQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<MarketSnapshot> Handle(SelectMarketSnapshotWithFilterQuery request, CancellationToken cancellationToken)
    {
        var queryParams = new SqlParams(request.MarketId, request.DateTime, (int)request.SnapshotType);

        var command = DatabaseQuery.Create(SqlCommand, queryParams, cancellationToken);

        var result = await _context.ExecuteFindAsync<MarketSnapshotEntity>(command);

        return result == null
            ? new MarketSnapshot(request.MarketId, request.SnapshotType, request.DateTime)
            : _mapper.Map<MarketSnapshot>(result);
    }

    private sealed class SqlParams
    {
        internal SqlParams(ulong marketId, DateTime date, int snapshotType)
        {
            MarketId = marketId;
            Date = date;
            SnapshotTypeId = snapshotType;
        }

        public ulong MarketId { get; }
        public DateTime Date { get; }
        public int SnapshotTypeId { get; }
    }
}