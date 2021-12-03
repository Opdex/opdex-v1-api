using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.MiningGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.MiningPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.LiquidityPools;

public class SelectLiquidityPoolsWithFilterQueryHandler : IRequestHandler<SelectLiquidityPoolsWithFilterQuery, IEnumerable<LiquidityPool>>
{
    private const string TableJoins = "{TableJoins}";
    private const string WhereFilter = "{WhereFilter}";
    private const string OrderBy = "{OrderBy}";
    private const string Limit = "{Limit}";

    private static readonly string SqlQuery =
        $@"SELECT DISTINCT
                pl.{nameof(LiquidityPoolEntity.Id)},
                pl.{nameof(LiquidityPoolEntity.Name)},
                pl.{nameof(LiquidityPoolEntity.Address)},
                pl.{nameof(LiquidityPoolEntity.SrcTokenId)},
                pl.{nameof(LiquidityPoolEntity.LpTokenId)},
                pl.{nameof(LiquidityPoolEntity.MarketId)},
                pl.{nameof(LiquidityPoolEntity.CreatedBlock)},
                pl.{nameof(LiquidityPoolEntity.ModifiedBlock)},
                pls.{nameof(LiquidityPoolSummaryEntity.LiquidityUsd)},
                pls.{nameof(LiquidityPoolSummaryEntity.VolumeUsd)},
                pls.{nameof(LiquidityPoolSummaryEntity.StakingWeight)}
            FROM pool_liquidity pl
            LEFT JOIN pool_liquidity_summary pls
                ON pl.{nameof(LiquidityPoolEntity.Id)} = pls.{nameof(LiquidityPoolSummaryEntity.LiquidityPoolId)}
            JOIN market m ON m.{nameof(MarketEntity.Id)} = pl.{nameof(LiquidityPoolEntity.MarketId)}
            {TableJoins}
            {WhereFilter}
            {OrderBy}
            {Limit}".RemoveExcessWhitespace();

    private const string InnerQuery = "{InnerQuery}";
    private const string OrderBySort = "{OrderBySort}";

    private static readonly string PagingBackwardQuery = @$"SELECT * FROM ({InnerQuery}) r {OrderBySort};";

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectLiquidityPoolsWithFilterQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<LiquidityPool>> Handle(SelectLiquidityPoolsWithFilterQuery request, CancellationToken cancellationToken)
    {
        var sqlParams = new SqlParams(request.Cursor.Keyword, request.Cursor.Markets, request.Cursor.LiquidityPools,
                                      request.Cursor.Tokens, request.Cursor.Pointer);

        var command = DatabaseQuery.Create(QueryBuilder(request), sqlParams, cancellationToken);

        var results = await _context.ExecuteQueryAsync<LiquidityPoolEntity>(command);

        return _mapper.Map<IEnumerable<LiquidityPool>>(results);
    }

    private static string QueryBuilder(SelectLiquidityPoolsWithFilterQuery request)
    {
        var whereFilter = string.Empty;
        var tableJoins = string.Empty;

        if (!request.Cursor.IsFirstRequest)
        {
            var sortOperator = string.Empty;

            // going forward in ascending order, use greater than
            if (request.Cursor.PagingDirection == PagingDirection.Forward && request.Cursor.SortDirection == SortDirectionType.ASC) sortOperator = ">";

            // going forward in descending order, use less than or equal to
            if (request.Cursor.PagingDirection == PagingDirection.Forward && request.Cursor.SortDirection == SortDirectionType.DESC) sortOperator = "<";

            // going backward in ascending order, use less than
            if (request.Cursor.PagingDirection == PagingDirection.Backward && request.Cursor.SortDirection == SortDirectionType.ASC) sortOperator = "<";

            // going backward in descending order, use greater than
            if (request.Cursor.PagingDirection == PagingDirection.Backward && request.Cursor.SortDirection == SortDirectionType.DESC) sortOperator = ">";

            var prefix = whereFilter.HasValue() ? " AND" : " WHERE";
            var suffix = $"pl.{nameof(LiquidityPoolEntity.Id)}) {sortOperator} (@{nameof(SqlParams.OrderByValue)}, @{nameof(SqlParams.LiquidityPoolId)})";

            whereFilter += request.Cursor.OrderBy switch
            {
                LiquidityPoolOrderByType.Liquidity => $"{prefix} (pls.{nameof(LiquidityPoolSummaryEntity.LiquidityUsd)}, {suffix}",
                LiquidityPoolOrderByType.Volume => $"{prefix} (pls.{nameof(LiquidityPoolSummaryEntity.VolumeUsd)}, {suffix}",
                LiquidityPoolOrderByType.StakingWeight => $"{prefix} (pls.{nameof(LiquidityPoolSummaryEntity.StakingWeight)}, {suffix}",
                LiquidityPoolOrderByType.Name => $"{prefix} (pl.{nameof(LiquidityPoolEntity.Name)}, {suffix}",
                _ => $"{prefix} pl.{nameof(LiquidityPoolEntity.Id)} {sortOperator} @{nameof(SqlParams.LiquidityPoolId)}"
            };
        }

        // Liquidity Pools filter
        if (request.Cursor.LiquidityPools.Any())
        {
            var prefix = whereFilter.HasValue() ? " AND" : " WHERE";
            whereFilter += $"{prefix} pl.{nameof(LiquidityPoolEntity.Address)} IN @{nameof(SqlParams.LiquidityPools)}";
        }

        // Markets filter
        if (request.Cursor.Markets.Any())
        {
            var prefix = whereFilter.HasValue() ? " AND" : " WHERE";
            whereFilter += $"{prefix} m.{nameof(MarketEntity.Address)} IN @{nameof(SqlParams.Markets)}";
        }

        // Tokens filter
        if (request.Cursor.Tokens.Any())
        {
            tableJoins += $" JOIN token t ON t.{nameof(TokenEntity.Id)} = pl.{nameof(LiquidityPoolEntity.SrcTokenId)}";

            var prefix = whereFilter.HasValue() ? " AND" : " WHERE";
            whereFilter += $"{prefix} t.{nameof(TokenEntity.Address)} IN @{nameof(SqlParams.Tokens)}";
        }

        // Keyword filter
        if (request.Cursor.Keyword.HasValue())
        {
            var prefix = whereFilter.HasValue() ? " AND" : " WHERE";
            whereFilter += @$"{prefix} (pl.{nameof(LiquidityPoolEntity.Name)} LIKE CONCAT('%', @{nameof(SqlParams.Keyword)}, '%') OR
                                        pl.{nameof(LiquidityPoolEntity.Address)} LIKE CONCAT('%', @{nameof(SqlParams.Keyword)}, '%'))";
        }

        // Mining filter
        if (request.Cursor.MiningFilter != LiquidityPoolMiningStatusFilter.Any)
        {
            tableJoins += $" JOIN pool_mining pm on pl.{nameof(LiquidityPoolEntity.Id)} = pm.{nameof(MiningPoolEntity.LiquidityPoolId)}";

            var conditional = request.Cursor.MiningFilter == LiquidityPoolMiningStatusFilter.Enabled ? ">=" : "<";
            var prefix = whereFilter.HasValue() ? " AND" : " WHERE";

            whereFilter += $@"{prefix} (pm.{nameof(MiningPoolEntity.MiningPeriodEndBlock)} {conditional}
                                (SELECT {nameof(BlockEntity.Height)} FROM block ORDER BY {nameof(BlockEntity.Height)} DESC LIMIT 1))";
        }

        // Staking filter
        if (request.Cursor.StakingFilter != LiquidityPoolStakingStatusFilter.Any)
        {
            var prefix = whereFilter.HasValue() ? " AND" : " WHERE";
            whereFilter += request.Cursor.StakingFilter == LiquidityPoolStakingStatusFilter.Enabled
                // IsStakingMarket && Pool.SrcTokenId != Market.StakingTokenId
                ? $@"{prefix} (m.{nameof(MarketEntity.StakingTokenId)} > 0 AND pl.SrcTokenId != m.{nameof(MarketEntity.StakingTokenId)})"
                // IsNotStakingMarket || Pool.SrcTokenId == Market.StakingTokenId
                : $@"{prefix} (m.{nameof(MarketEntity.StakingTokenId)} = 0
                            OR (m.{nameof(MarketEntity.StakingTokenId)} > 0
                                AND pl.{nameof(LiquidityPoolEntity.SrcTokenId)} = m.{nameof(MarketEntity.StakingTokenId)}))";
        }

        // Nominated filter
        if (request.Cursor.NominationFilter != LiquidityPoolNominationStatusFilter.Any)
        {
            tableJoins += $@" JOIN mining_governance_nomination gn ON gn.{nameof(MiningGovernanceNominationEntity.LiquidityPoolId)} = pl.{nameof(LiquidityPoolEntity.Id)}";

            var prefix = whereFilter.HasValue() ? " AND" : " WHERE";
            var status = request.Cursor.NominationFilter == LiquidityPoolNominationStatusFilter.Nominated ? "true" : "false";

            whereFilter += $"{prefix} gn.{nameof(MiningGovernanceNominationEntity.IsNominated)} = {status}";
        }

        // Set the direction, moving backwards with previous requests, the sort order must be reversed first.
        string direction;

        if (request.Cursor.PagingDirection == PagingDirection.Backward)
            direction = request.Cursor.SortDirection == SortDirectionType.DESC ? nameof(SortDirectionType.ASC) : nameof(SortDirectionType.DESC);
        else
            direction = Enum.GetName(typeof(SortDirectionType), request.Cursor.SortDirection);

        // Order the rows by the preferred, indexed column or tokenId by default
        var orderBy = OrderByBuilder(request.Cursor.OrderBy, direction, reverse: false);

        var limit = $" LIMIT {request.Cursor.Limit + 1}";

        var query = SqlQuery.Replace(WhereFilter, whereFilter)
            .Replace(TableJoins, tableJoins)
            .Replace(OrderBy, orderBy)
            .Replace(Limit, limit).RemoveExcessWhitespace();

        if (request.Cursor.PagingDirection == PagingDirection.Forward) return $"{query};";

        // re-sort back into requested order
        return PagingBackwardQuery.Replace(InnerQuery, query)
            .Replace(OrderBySort, OrderByBuilder(request.Cursor.OrderBy,
                                                 Enum.GetName(typeof(SortDirectionType), request.Cursor.SortDirection),
                                                 reverse: true));
    }

    private static string OrderByBuilder(LiquidityPoolOrderByType cursorOrderBy, string direction, bool reverse)
    {
        var summaryPrefix = reverse ? "r" : "pls";
        var poolPrefix = reverse ? "r" : "pl";

        var lpIdWithDirection = $"{poolPrefix}.{nameof(LiquidityPoolEntity.Id)} {direction}";

        return cursorOrderBy switch
        {
            LiquidityPoolOrderByType.Liquidity => $" ORDER BY {summaryPrefix}.{nameof(LiquidityPoolSummaryEntity.LiquidityUsd)} {direction}, {lpIdWithDirection}",
            LiquidityPoolOrderByType.Volume => $" ORDER BY {summaryPrefix}.{nameof(LiquidityPoolSummaryEntity.VolumeUsd)} {direction}, {lpIdWithDirection}",
            LiquidityPoolOrderByType.StakingWeight => $" ORDER BY {summaryPrefix}.{nameof(LiquidityPoolSummaryEntity.StakingWeight)} {direction}, {lpIdWithDirection}",
            LiquidityPoolOrderByType.Name => $" ORDER BY {poolPrefix}.{nameof(LiquidityPoolEntity.Name)} {direction}, {lpIdWithDirection}",
            _ => $" ORDER BY {lpIdWithDirection}"
        };
    }

    private sealed class SqlParams
    {
        internal SqlParams(string keyword, IEnumerable<Address> markets, IEnumerable<Address> pools, IEnumerable<Address> tokens, (string, ulong) pointer)
        {
            Keyword = keyword;
            OrderByValue = pointer.Item1;
            LiquidityPoolId = pointer.Item2;
            Markets = markets.Select(market => market.ToString());
            LiquidityPools = pools.Select(pool => pool.ToString());
            Tokens = tokens.Select(pool => pool.ToString());
        }

        public string Keyword { get; }
        public string OrderByValue { get; }
        public ulong LiquidityPoolId { get; }
        public IEnumerable<string> Markets { get; }
        public IEnumerable<string> LiquidityPools { get; }
        public IEnumerable<string> Tokens { get; }
    }
}