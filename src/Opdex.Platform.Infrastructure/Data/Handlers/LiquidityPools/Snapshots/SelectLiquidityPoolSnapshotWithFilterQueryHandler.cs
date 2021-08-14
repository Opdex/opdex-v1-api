using AutoMapper;
using MediatR;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.LiquidityPools.Snapshots;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools.Snapshots;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.LiquidityPools.Snapshots
{
    public class SelectLiquidityPoolSnapshotWithFilterQueryHandler
        : IRequestHandler<SelectLiquidityPoolSnapshotWithFilterQuery, LiquidityPoolSnapshot>
    {
        private static readonly string SqlQuery =
            @$"SELECT
                {nameof(LiquidityPoolSnapshotEntity.Id)},
                {nameof(LiquidityPoolSnapshotEntity.LiquidityPoolId)},
                {nameof(LiquidityPoolSnapshotEntity.SnapshotTypeId)},
                {nameof(LiquidityPoolSnapshotEntity.TransactionCount)},
                {nameof(LiquidityPoolSnapshotEntity.Details)},
                {nameof(LiquidityPoolSnapshotEntity.StartDate)},
                {nameof(LiquidityPoolSnapshotEntity.EndDate)},
                {nameof(LiquidityPoolSnapshotEntity.ModifiedDate)}
            FROM pool_liquidity_snapshot
            WHERE {nameof(LiquidityPoolSnapshotEntity.LiquidityPoolId)} = @{nameof(SqlParams.LiquidityPoolId)}
                AND
                    (
                        @{nameof(SqlParams.Date)} BETWEEN
                            {nameof(LiquidityPoolSnapshotEntity.StartDate)} AND {nameof(LiquidityPoolSnapshotEntity.EndDate)}
                        OR
                        @{nameof(SqlParams.Date)} > {nameof(LiquidityPoolSnapshotEntity.EndDate)}
                    )
                AND {nameof(LiquidityPoolSnapshotEntity.SnapshotTypeId)} = @{nameof(SqlParams.SnapshotTypeId)}
            ORDER BY {nameof(LiquidityPoolSnapshotEntity.EndDate)} DESC
            LIMIT 1;";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectLiquidityPoolSnapshotWithFilterQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<LiquidityPoolSnapshot> Handle(SelectLiquidityPoolSnapshotWithFilterQuery request, CancellationToken cancellationToken)
        {
            var queryParams = new SqlParams(request.LiquidityPoolId, request.Date, (int)request.SnapshotType);

            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationToken);

            var result = await _context.ExecuteFindAsync<LiquidityPoolSnapshotEntity>(query);

            return result == null
                ? new LiquidityPoolSnapshot(request.LiquidityPoolId, request.SnapshotType, request.Date)
                : _mapper.Map<LiquidityPoolSnapshot>(result);
        }

        private sealed class SqlParams
        {
            internal SqlParams(long liquidityPoolId, DateTime date, int snapshotType)
            {
                LiquidityPoolId = liquidityPoolId;
                Date = date;
                SnapshotTypeId = snapshotType;
            }

            public long LiquidityPoolId { get; }
            public DateTime Date { get; }
            public int SnapshotTypeId { get; }
        }
    }
}
