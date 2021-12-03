using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools.Summaries;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.LiquidityPools.Summaries;

public class SelectLiquidityPoolSummaryByLiquidityPoolIdQueryHandler
    : IRequestHandler<SelectLiquidityPoolSummaryByLiquidityPoolIdQuery, LiquidityPoolSummary>
{
    private static readonly string SqlQuery =
        @$"SELECT
                {nameof(LiquidityPoolSummaryEntity.Id)},
                {nameof(LiquidityPoolSummaryEntity.LiquidityPoolId)},
                {nameof(LiquidityPoolSummaryEntity.LiquidityUsd)},
                {nameof(LiquidityPoolSummaryEntity.DailyLiquidityUsdChangePercent)},
                {nameof(LiquidityPoolSummaryEntity.VolumeUsd)},
                {nameof(LiquidityPoolSummaryEntity.StakingWeight)},
                {nameof(LiquidityPoolSummaryEntity.DailyStakingWeightChangePercent)},
                {nameof(LiquidityPoolSummaryEntity.LockedCrs)},
                {nameof(LiquidityPoolSummaryEntity.LockedSrc)},
                {nameof(LiquidityPoolSummaryEntity.CreatedBlock)},
                {nameof(LiquidityPoolSummaryEntity.ModifiedBlock)}
            FROM pool_liquidity_summary
            WHERE {nameof(LiquidityPoolSummaryEntity.LiquidityPoolId)} = @{nameof(SqlParams.LiquidityPoolId)}
            LIMIT 1;";

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectLiquidityPoolSummaryByLiquidityPoolIdQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<LiquidityPoolSummary> Handle(SelectLiquidityPoolSummaryByLiquidityPoolIdQuery request, CancellationToken cancellationToken)
    {
        var queryParams = new SqlParams(request.LiquidityPoolId);
        var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationToken);

        var result = await _context.ExecuteFindAsync<LiquidityPoolSummaryEntity>(query);

        if (request.FindOrThrow && result == null)
        {
            throw new NotFoundException($"{nameof(LiquidityPoolSummary)} not found.");
        }

        return result == null ? null : _mapper.Map<LiquidityPoolSummary>(result);
    }

    private sealed class SqlParams
    {
        internal SqlParams(ulong liquidityPoolId)
        {
            LiquidityPoolId = liquidityPoolId;
        }

        public ulong LiquidityPoolId { get; }
    }
}