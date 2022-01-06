using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Markets;

public class SelectMarketsWithFilterQueryHandler : IRequestHandler<SelectMarketsWithFilterQuery, IEnumerable<Market>>
{
    private const string PrimaryOrderColumn = "{PrimaryOrderColumn}";
    private const string TableJoins = "{TableJoins}";
    private const string WhereFilter = "{WhereFilter}";
    private const string OrderBy = "{OrderBy}";
    private const string Limit = "{Limit}";

    private static readonly string SqlQuery =
        $@"SELECT
                {PrimaryOrderColumn}
                m.{nameof(MarketEntity.Id)},
                m.{nameof(MarketEntity.Address)},
                m.{nameof(MarketEntity.DeployerId)},
                m.{nameof(MarketEntity.StakingTokenId)},
                m.{nameof(MarketEntity.PendingOwner)},
                m.{nameof(MarketEntity.Owner)},
                m.{nameof(MarketEntity.AuthPoolCreators)},
                m.{nameof(MarketEntity.AuthProviders)},
                m.{nameof(MarketEntity.AuthTraders)},
                m.{nameof(MarketEntity.TransactionFee)},
                m.{nameof(MarketEntity.MarketFeeEnabled)},
                m.{nameof(MarketEntity.CreatedBlock)},
                m.{nameof(MarketEntity.ModifiedBlock)}
            FROM market m
            {TableJoins}
            {WhereFilter}
            {OrderBy}
            {Limit}".RemoveExcessWhitespace();

    private const string InnerQuery = "{InnerQuery}";
    private const string OrderBySort = "{OrderBySort}";

    private static readonly string PagingBackwardQuery = @$"SELECT * FROM ({InnerQuery}) r {OrderBySort};";

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectMarketsWithFilterQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<Market>> Handle(SelectMarketsWithFilterQuery request, CancellationToken cancellationToken)
    {
        var sqlParams = new SqlParams(request.Cursor.Pointer);

        var command = DatabaseQuery.Create(QueryBuilder(request), sqlParams, cancellationToken);

        var results = await _context.ExecuteQueryAsync<MarketEntity>(command);

        return _mapper.Map<IEnumerable<Market>>(results);
    }

    private static string QueryBuilder(SelectMarketsWithFilterQuery request)
    {
        var whereFilterBuilder = new WhereFilterBuilder();
        var tableJoins = string.Empty;
        var primaryOrderColumn = string.Empty;

        if (!request.Cursor.IsFirstRequest)
        {
            var sortOperator = request.Cursor.PagingDirection switch
            {
                // going backward in descending order, use greater than
                PagingDirection.Backward when request.Cursor.SortDirection == SortDirectionType.DESC => ">",
                // going backward in ascending order, use less than
                PagingDirection.Backward when request.Cursor.SortDirection == SortDirectionType.ASC => "<",
                // going forward in descending order, use less than or equal to
                PagingDirection.Forward when request.Cursor.SortDirection == SortDirectionType.DESC => "<",
                // going forward in ascending order, use greater than
                PagingDirection.Forward when request.Cursor.SortDirection == SortDirectionType.ASC => ">",
                _ => string.Empty
            };

            var suffix = $"m.{nameof(MarketEntity.Id)}) {sortOperator} (@{nameof(SqlParams.OrderByPointer)}, @{nameof(SqlParams.MarketIdPointer)})";

            var pointerCondition = request.Cursor.OrderBy switch
            {
                // we coalesce as market summary may be null, if the market has recently been created
                MarketOrderByType.LiquidityUsd => $"(COALESCE(ms.{nameof(MarketSummaryEntity.LiquidityUsd)}, 0), {suffix}",
                MarketOrderByType.StakingUsd => $"(COALESCE(ms.{nameof(MarketSummaryEntity.StakingUsd)}, 0), {suffix}",
                MarketOrderByType.StakingWeight => $"(COALESCE(ms.{nameof(MarketSummaryEntity.StakingWeight)}, 0), {suffix}",
                MarketOrderByType.VolumeUsd => $"(COALESCE(ms.{nameof(MarketSummaryEntity.VolumeUsd)}, 0), {suffix}",
                MarketOrderByType.MarketRewardsDailyUsd => $"(COALESCE(ms.{nameof(MarketSummaryEntity.MarketRewardsDailyUsd)}, 0), {suffix}",
                MarketOrderByType.ProviderRewardsDailyUsd => $"(COALESCE(ms.{nameof(MarketSummaryEntity.ProviderRewardsDailyUsd)}, 0), {suffix}",
                MarketOrderByType.DailyLiquidityUsdChangePercent => $"(COALESCE(ms.{nameof(MarketSummaryEntity.DailyLiquidityUsdChangePercent)}, 0), {suffix}",
                MarketOrderByType.DailyStakingUsdChangePercent => $"(COALESCE(ms.{nameof(MarketSummaryEntity.DailyStakingUsdChangePercent)}, 0), {suffix}",
                MarketOrderByType.DailyStakingWeightChangePercent => $"(COALESCE(ms.{nameof(MarketSummaryEntity.DailyStakingWeightChangePercent)}, 0), {suffix}",
                _ => $"m.{nameof(MarketEntity.Id)} {sortOperator} @{nameof(SqlParams.MarketIdPointer)}"
            };

            whereFilterBuilder.AppendCondition(pointerCondition);
        }

        // market type filter
        if (request.Cursor.Type != MarketType.All)
        {
            var stakingTokenIdOperator = request.Cursor.Type == MarketType.Standard ? '=' : '>';
            whereFilterBuilder.AppendCondition($"m.{nameof(MarketEntity.StakingTokenId)} {stakingTokenIdOperator} 0");
        }

        if (request.Cursor.OrderBy != MarketOrderByType.Default)
        {
            tableJoins += " LEFT JOIN market_summary ms ON m.Id = ms.MarketId";
            var columnName = request.Cursor.OrderBy switch
            {
                MarketOrderByType.LiquidityUsd => nameof(MarketSummary.LiquidityUsd),
                MarketOrderByType.StakingUsd => nameof(MarketSummary.StakingUsd),
                MarketOrderByType.StakingWeight => nameof(MarketSummary.StakingWeight),
                MarketOrderByType.VolumeUsd => nameof(MarketSummary.VolumeUsd),
                MarketOrderByType.MarketRewardsDailyUsd => nameof(MarketSummary.MarketRewardsDailyUsd),
                MarketOrderByType.ProviderRewardsDailyUsd => nameof(MarketSummary.ProviderRewardsDailyUsd),
                MarketOrderByType.DailyLiquidityUsdChangePercent => nameof(MarketSummary.DailyLiquidityUsdChangePercent),
                MarketOrderByType.DailyStakingUsdChangePercent => nameof(MarketSummary.DailyStakingUsdChangePercent),
                MarketOrderByType.DailyStakingWeightChangePercent => nameof(MarketSummary.DailyStakingWeightChangePercent),
                _ => throw new InvalidOperationException()
            };

            primaryOrderColumn = $"COALESCE(ms.{columnName}, 0) AS {columnName},";
        }

        // Set the direction, moving backwards with previous requests, the sort order must be reversed first.
        string direction = request.Cursor.PagingDirection switch
        {
            PagingDirection.Backward => request.Cursor.SortDirection == SortDirectionType.DESC
                ? nameof(SortDirectionType.ASC)
                : nameof(SortDirectionType.DESC),
            _ => Enum.GetName(typeof(SortDirectionType), request.Cursor.SortDirection)
        };

        var orderBy = OrderByBuilder(request.Cursor.OrderBy, direction, false);

        var limit = $" LIMIT {request.Cursor.Limit + 1}";

        var query = SqlQuery
            .Replace(PrimaryOrderColumn, primaryOrderColumn)
            .Replace(WhereFilter, whereFilterBuilder.ToString())
            .Replace(TableJoins, tableJoins)
            .Replace(OrderBy, orderBy)
            .Replace(Limit, limit).RemoveExcessWhitespace();

        return request.Cursor.PagingDirection == PagingDirection.Forward
            ? $"{query};"
            : PagingBackwardQuery.Replace(InnerQuery, query)
                                 .Replace(OrderBySort, OrderByBuilder(request.Cursor.OrderBy,
                                     Enum.GetName(typeof(SortDirectionType), request.Cursor.SortDirection),
                                     reverse: true));

    }

    private static string OrderByBuilder(MarketOrderByType cursorOrderBy, string direction, bool reverse)
    {
        var summaryPrefix = reverse ? "r" : "ms";
        var marketPrefix = reverse ? "r" : "m";

        var sortPart = cursorOrderBy switch
        {
            // we coalesce as market summary may be null, if the market has recently been created
            MarketOrderByType.LiquidityUsd => $"COALESCE({summaryPrefix}.{nameof(MarketSummaryEntity.LiquidityUsd)}, 0) {direction},",
            MarketOrderByType.StakingUsd => $"COALESCE({summaryPrefix}.{nameof(MarketSummaryEntity.StakingUsd)}, 0) {direction},",
            MarketOrderByType.StakingWeight => $"COALESCE({summaryPrefix}.{nameof(MarketSummaryEntity.StakingWeight)}, 0) {direction},",
            MarketOrderByType.VolumeUsd => $"COALESCE({summaryPrefix}.{nameof(MarketSummaryEntity.VolumeUsd)}, 0) {direction},",
            MarketOrderByType.MarketRewardsDailyUsd => $"COALESCE({summaryPrefix}.{nameof(MarketSummaryEntity.MarketRewardsDailyUsd)}, 0) {direction},",
            MarketOrderByType.ProviderRewardsDailyUsd => $"COALESCE({summaryPrefix}.{nameof(MarketSummaryEntity.ProviderRewardsDailyUsd)}, 0) {direction},",
            MarketOrderByType.DailyLiquidityUsdChangePercent => $"COALESCE({summaryPrefix}.{nameof(MarketSummaryEntity.DailyLiquidityUsdChangePercent)}, 0) {direction},",
            MarketOrderByType.DailyStakingUsdChangePercent => $"COALESCE({summaryPrefix}.{nameof(MarketSummaryEntity.DailyStakingUsdChangePercent)}, 0) {direction},",
            MarketOrderByType.DailyStakingWeightChangePercent => $"COALESCE({summaryPrefix}.{nameof(MarketSummaryEntity.DailyStakingWeightChangePercent)}, 0) {direction},",
            _ => ""
        };

        var stringBuilder = new StringBuilder();
        stringBuilder.Append(" ORDER BY ").Append(sortPart);
        stringBuilder.Append(' ').Append(marketPrefix).Append('.').Append(nameof(MarketEntity.Id)).Append(' ').Append(direction);

        return stringBuilder.ToString();
    }

    class SqlParams
    {
        public SqlParams((FixedDecimal, ulong) pointer)
        {
            (FixedDecimal orderPart, ulong marketIdPart) = pointer;

            OrderByPointer = orderPart;
            MarketIdPointer = marketIdPart;
        }

        public FixedDecimal OrderByPointer { get; }
        public ulong MarketIdPointer { get; }
    }
}
