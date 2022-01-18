using AutoMapper;
using MediatR;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.LiquidityPools;

public class SelectLiquidityPoolsBySummaryModifiedBlockThresholdQueryHandler
    : IRequestHandler<SelectLiquidityPoolsBySummaryModifiedBlockThresholdQuery, IEnumerable<LiquidityPool>>
{
    private const string LatestBlockQuery = $"SELECT Height FROM block ORDER BY {nameof(BlockEntity.Height)} DESC LIMIT 1";
    private const string SqlQuery =
        @$"SELECT
                pl.{nameof(LiquidityPoolEntity.Id)},
                pl.{nameof(LiquidityPoolEntity.Name)},
                pl.{nameof(LiquidityPoolEntity.Address)},
                pl.{nameof(LiquidityPoolEntity.SrcTokenId)},
                pl.{nameof(LiquidityPoolEntity.LpTokenId)},
                pl.{nameof(LiquidityPoolEntity.MarketId)},
                pl.{nameof(LiquidityPoolEntity.CreatedBlock)},
                pl.{nameof(LiquidityPoolEntity.ModifiedBlock)}
            FROM pool_liquidity pl
                JOIN pool_liquidity_summary pls ON pls.{nameof(LiquidityPoolSummaryEntity.LiquidityPoolId)} = pl.{nameof(LiquidityPoolEntity.Id)}
            WHERE (({LatestBlockQuery}) - @{nameof(SqlParams.BlockThreshold)}) > pls.{nameof(LiquidityPoolSummaryEntity.ModifiedBlock)};";

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectLiquidityPoolsBySummaryModifiedBlockThresholdQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<LiquidityPool>> Handle(SelectLiquidityPoolsBySummaryModifiedBlockThresholdQuery request, CancellationToken cancellationToken)
    {
        var queryParams = new SqlParams(request.BlockThreshold);

        var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationToken);

        var result = await _context.ExecuteFindAsync<LiquidityPoolEntity>(query);

        return _mapper.Map<IEnumerable<LiquidityPool>>(result);
    }

    private sealed class SqlParams
    {
        internal SqlParams(ulong blockThreshold)
        {
            BlockThreshold = blockThreshold;
        }

        public ulong BlockThreshold { get; }
    }
}
