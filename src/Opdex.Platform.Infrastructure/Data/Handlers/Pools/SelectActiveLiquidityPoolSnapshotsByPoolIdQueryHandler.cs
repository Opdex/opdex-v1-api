using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pools;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Pools
{
    public class SelectActiveLiquidityPoolSnapshotsByPoolIdQueryHandler 
        : IRequestHandler<SelectActiveLiquidityPoolSnapshotsByPoolIdQuery, IEnumerable<LiquidityPoolSnapshot>>
    {
        private static readonly string SqlQuery =
            @$"Select 
                {nameof(LiquidityPoolSnapshotEntity.Id)},
                {nameof(LiquidityPoolSnapshotEntity.PoolId)},
                {nameof(LiquidityPoolSnapshotEntity.TransactionCount)},
                {nameof(LiquidityPoolSnapshotEntity.ReserveCrs)},
                {nameof(LiquidityPoolSnapshotEntity.ReserveSrc)},
                {nameof(LiquidityPoolSnapshotEntity.ReserveUsd)},
                {nameof(LiquidityPoolSnapshotEntity.VolumeCrs)},
                {nameof(LiquidityPoolSnapshotEntity.VolumeSrc)},
                {nameof(LiquidityPoolSnapshotEntity.VolumeUsd)},
                {nameof(LiquidityPoolSnapshotEntity.StakingWeight)},
                {nameof(LiquidityPoolSnapshotEntity.StakingUsd)},
                {nameof(LiquidityPoolSnapshotEntity.SnapshotType)},
                {nameof(LiquidityPoolSnapshotEntity.StartDate)},
                {nameof(LiquidityPoolSnapshotEntity.EndDate)}
            FROM market_snapshot
            WHERE {nameof(LiquidityPoolSnapshotEntity.PoolId)} = @{nameof(SqlParams.PoolId)}
                AND @{nameof(SqlParams.Time)} BETWEEN 
                    {nameof(LiquidityPoolSnapshotEntity.StartDate)} AND {nameof(LiquidityPoolSnapshotEntity.EndDate)};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectActiveLiquidityPoolSnapshotsByPoolIdQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<LiquidityPoolSnapshot>> Handle(SelectActiveLiquidityPoolSnapshotsByPoolIdQuery request, CancellationToken cancellationToken)
        {
            var queryParams = new SqlParams(request.PoolId, request.Time);
            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationToken);

            var result = await _context.ExecuteQueryAsync<LiquidityPoolSnapshotEntity>(query);

            return _mapper.Map<IEnumerable<LiquidityPoolSnapshot>>(result);
        }

        private sealed class SqlParams
        {
            internal SqlParams(long tokenId, DateTime time)
            {
                PoolId = tokenId;
                Time = time;
            }

            public long PoolId { get; }
            public DateTime Time { get; }
        }
    }
}