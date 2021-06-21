using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Domain.Models.Pools.Snapshots;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Pools.Snapshots;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pools.Snapshots;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Pools.Snapshots
{
    public class SelectLiquidityPoolSnapshotsWithFilterQueryHandler
        : IRequestHandler<SelectLiquidityPoolSnapshotsWithFilterQuery, IEnumerable<LiquidityPoolSnapshot>>
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
                AND @{nameof(SqlParams.Start)} <= {nameof(LiquidityPoolSnapshotEntity.StartDate)} 
                AND @{nameof(SqlParams.End)} >= {nameof(LiquidityPoolSnapshotEntity.EndDate)}
                AND {nameof(LiquidityPoolSnapshotEntity.SnapshotTypeId)} = @{nameof(SqlParams.SnapshotTypeId)}
            ORDER BY {nameof(LiquidityPoolSnapshotEntity.EndDate)} DESC
            LIMIT 750;"; // Limit 750, there's about 730 hours in a month (hourly snapshots)

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectLiquidityPoolSnapshotsWithFilterQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<LiquidityPoolSnapshot>> Handle(SelectLiquidityPoolSnapshotsWithFilterQuery request, CancellationToken cancellationToken)
        {
            var queryParams = new SqlParams(request.PoolId, request.StartDate, request.EndDate, (int)request.SnapshotType);

            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationToken);

            var result = await _context.ExecuteQueryAsync<LiquidityPoolSnapshotEntity>(query);

            return _mapper.Map<IEnumerable<LiquidityPoolSnapshot>>(result);
        }

        private sealed class SqlParams
        {
            internal SqlParams(long poolId, DateTime start, DateTime end, int snapshotTypeId)
            {
                LiquidityPoolId = poolId;
                Start = start;
                End = end;
                SnapshotTypeId = snapshotTypeId;
            }

            public long LiquidityPoolId { get; }
            public DateTime Start { get; }
            public DateTime End { get; }
            public int SnapshotTypeId { get; }
        }
    }
}