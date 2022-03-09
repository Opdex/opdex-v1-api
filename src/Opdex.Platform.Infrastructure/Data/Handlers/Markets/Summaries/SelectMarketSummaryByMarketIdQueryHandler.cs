using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets.Summaries;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Markets.Summaries;

public class SelectMarketSummaryByMarketIdQueryHandler
    : IRequestHandler<SelectMarketSummaryByMarketIdQuery, MarketSummary>
{
    private static readonly string SqlQuery =
        @$"SELECT
                {nameof(MarketSummaryEntity.Id)},
                {nameof(MarketSummaryEntity.MarketId)},
                {nameof(MarketSummaryEntity.LiquidityUsd)},
                {nameof(MarketSummaryEntity.DailyLiquidityUsdChangePercent)},
                {nameof(MarketSummaryEntity.VolumeUsd)},
                {nameof(MarketSummaryEntity.StakingWeight)},
                {nameof(MarketSummaryEntity.DailyStakingWeightChangePercent)},
                {nameof(MarketSummaryEntity.StakingUsd)},
                {nameof(MarketSummaryEntity.DailyStakingUsdChangePercent)},
                {nameof(MarketSummaryEntity.ProviderRewardsDailyUsd)},
                {nameof(MarketSummaryEntity.MarketRewardsDailyUsd)},
                {nameof(MarketSummaryEntity.LiquidityPoolCount)},
                {nameof(MarketSummaryEntity.CreatedBlock)},
                {nameof(MarketSummaryEntity.ModifiedBlock)}
            FROM market_summary
            WHERE {nameof(MarketSummaryEntity.MarketId)} = @{nameof(SqlParams.MarketId)}
            LIMIT 1;".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectMarketSummaryByMarketIdQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<MarketSummary> Handle(SelectMarketSummaryByMarketIdQuery request, CancellationToken cancellationToken)
    {
        var queryParams = new SqlParams(request.MarketId);
        var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationToken);

        var result = await _context.ExecuteFindAsync<MarketSummaryEntity>(query);

        if (request.FindOrThrow && result == null)
        {
            throw new NotFoundException("Market summary not found.");
        }

        return result == null ? null : _mapper.Map<MarketSummary>(result);
    }

    private sealed class SqlParams
    {
        internal SqlParams(ulong marketId)
        {
            MarketId = marketId;
        }

        public ulong MarketId { get; }
    }
}
