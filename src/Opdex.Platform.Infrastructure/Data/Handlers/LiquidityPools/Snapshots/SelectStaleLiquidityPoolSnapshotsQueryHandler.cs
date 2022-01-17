using AutoMapper;
using MediatR;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.LiquidityPools.Snapshots;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools.Snapshots;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.LiquidityPools.Snapshots;

public class SelectStaleLiquidityPoolSnapshotsQueryHandler : IRequestHandler<SelectStaleLiquidityPoolSnapshotsQuery, IEnumerable<LiquidityPoolSnapshot>>
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
            WHERE UTC_TIMESTAMP BETWEEN {nameof(LiquidityPoolSnapshotEntity.StartDate)} AND {nameof(LiquidityPoolSnapshotEntity.EndDate)}
                AND {nameof(LiquidityPoolSnapshotEntity.ModifiedDate)} > DATE_SUB(UTC_TIMESTAMP, INTERVAL 15 MINUTE);".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectStaleLiquidityPoolSnapshotsQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<LiquidityPoolSnapshot>> Handle(SelectStaleLiquidityPoolSnapshotsQuery request, CancellationToken cancellationToken)
    {
        var query = DatabaseQuery.Create(SqlQuery, cancellationToken);

        var result = await _context.ExecuteQueryAsync<LiquidityPoolSnapshotEntity>(query);

        return _mapper.Map<IEnumerable<LiquidityPoolSnapshot>>(result);
    }
}
